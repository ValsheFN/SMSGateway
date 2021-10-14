using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SMSGateway.Repositories;
using SMSGateway.Server.Infrastructure;
using SMSGateway.Server.Models.Models;
using SMSGateway.Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SMSGateway.Server.Services
{
    public interface IUsersService
    {
        Task<OperationResponse<string>> RegisterUserAsync(RegisterRequest model);
        Task<LoginResponse> GenerateTokenAsync(LoginRequest model);
        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);
        Task<UserManagerResponse> ForgetPasswordAsync(string email);
        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordRequest model);

    }

    public class UsersService : IUsersService
    {
        private UserManager<ApplicationUser> _userManager;
        private IMailService _mailService;
        private IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        public readonly AuthOptions _authOptions;

        public UsersService(UserManager<ApplicationUser> userManager,
                            IMailService mailService,
                            IConfiguration configuration,
                            IUnitOfWork unitOfWork, 
                            AuthOptions authOptions)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
            _unitOfWork = unitOfWork;
            _authOptions = authOptions;
        }

        public async Task<LoginResponse> GenerateTokenAsync(LoginRequest model)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(model.Email);
            if(user == null)
            {
                return new LoginResponse
                {
                    Message = "Invalid username or password",
                    IsSuccess = false
                };
            }

            if (!(await _unitOfWork.Users.CheckPasswordAsync(user, model.Password)))
            {
                return new LoginResponse
                {
                    Message = "Invalid username or password",
                    IsSuccess = false
                };
            }

            var userRole = await _unitOfWork.Users.GetUserRoleAsync(user);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, userRole)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.Key));
            var expireDate = DateTime.Now.AddDays(30);
            var token = new JwtSecurityToken(
                issuer: _authOptions.Issuer,
                audience: _authOptions.Audience,
                claims: claims,
                expires: expireDate,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse
            {
                Message = "Login successful",
                IsSuccess = true,
                AccessToken = tokenAsString,
                ExpiryDate = expireDate
            };
        }

        public async Task<OperationResponse<string>> RegisterUserAsync(RegisterRequest model)
        {
            var userEmail = await _unitOfWork.Users.GetUserByEmailAsync(model.Email);
            if(userEmail != null)
            {
                return new OperationResponse<string>
                {
                    IsSuccess = false,
                    Message = "User is already exists"
                };
            }

           /* if(model.Password != null)
            {

            }*/

            //Check whether username is null
            var userName = "";

            if (model.UserName == null)
            {
                userName = model.Email;
            }
            else
            {
                userName = model.UserName;
            }

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = userName
            };

            //Verify Password

            var passwordError = "";
                var regexItem = new Regex("^[a-zA-Z0-9 ]*$");

                if (!(model.Password.Any(char.IsUpper)))
                {
                     passwordError = "Password must contain at least 1 uppercase character";
                }
                else if (!(model.Password.Any(char.IsLower)))
                {
                    passwordError = "Password must contain at least 1 lowercase character";
                }
                else if (!(model.Password.Any(char.IsDigit)))
                {
                    passwordError = "Password must contain at least 1 number";
                }
                else if ((regexItem.IsMatch(model.Password)))
                {
                    passwordError = "Password must contain at least 1 special character";
                }

            if (passwordError.Length == 0)
            {
                await _unitOfWork.Users.CreateUserAsync(user, model.Password, "User");
            }
            else
            {
                return new OperationResponse<string>
                {
                    Message = passwordError,
                    IsSuccess = false
                };
            }

            try
            {
                //Generate email confirmation token
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //Generate byte array for the token
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}/api/authentication/confirmemail?userId={user.Id}&token={validEmailToken}";

                //Send email confirmation
                await _mailService.SendEmailAsync(model.Email, "Confirm your email", "<h1>Welcome to Auth Demo</h1>" +
                    $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");

                return new OperationResponse<string>
                {
                    Message = "User created successfully",
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                return new OperationResponse<string>
                {
                    Message = e.ToString(),
                    IsSuccess = false
                };
            }
        }

        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _unitOfWork.Users.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            }

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "Email confirmed successfully",
                    IsSuccess = true
                };
            }

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Cannot confirm email",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with such email"
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/resetpassword?email={email}&token={validToken}";

            await _mailService.SendEmailAsync(email, "Reset Password", "<h1>Follow the instructions to reset your password</h1>" +
                $"<p>To reset your password <a href='{url}'>Click here</a></p>");

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Reset password URL has been sent to the email successfully!"
            };
        }
        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email",
                };

            if (model.NewPassword != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Confirmation password doesn't match the password",
                };

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Password has been reset successfully!",
                    IsSuccess = true,
                };

            return new UserManagerResponse
            {
                Message = "Something went wrong",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }
    }
}
