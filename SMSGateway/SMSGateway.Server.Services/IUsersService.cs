using Microsoft.IdentityModel.Tokens;
using SMSGateway.Repositories;
using SMSGateway.Server.Infrastructure;
using SMSGateway.Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway.Server.Services
{
    public interface IUsersService
    {
        //Task RegisterUserAsync();

        Task<object> GenerateTokenAsync(LoginRequest model);
    }

    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _unitOfWork;
        public readonly AuthOptions _authOptions;

        public UsersService(IUnitOfWork unitOfWork, AuthOptions authOptions)
        {
            _unitOfWork = unitOfWork;
            _authOptions = authOptions;
        }

        public async Task<object> GenerateTokenAsync(LoginRequest model)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(model.Email);
            if(user == null)
            {
                //TODO: Return response when user is null
                return null;
            }

            if(await _unitOfWork.Users.CheckPasswordAsync(user, model.Password))
            {
                //TODO: Return response when password is incorrect
                return null;
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

            return new
            {
                IsSuccess = true,
                AccessToken = tokenAsString,
                ExpiryDate = expireDate
            };
        }
    }
}
