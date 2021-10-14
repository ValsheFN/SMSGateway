using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SMSGateway.Server.Services;
using SMSGateway.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSGateway.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IUsersService _usersService;
        private IMailService _mailService;
        private IConfiguration _configuration;

        public AuthenticationController(IUsersService usersService, IMailService mailService, IConfiguration configuration)
        {
            _usersService = usersService;
            _mailService = mailService;
            _configuration = configuration;
        }

        // /api/authentication/register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (ModelState.IsValid)
            {
                var result = await _usersService.RegisterUserAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Internal Server Error"); //Status code : 400
        }

        // /api/authentication/login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (ModelState.IsValid)
            {
                var result = await _usersService.GenerateTokenAsync(model);
                if (result.IsSuccess)
                {
                    await _mailService.SendEmailAsync(model.Email, "New login", "<h1>New login to your account noticed<h1><p>New login to your account at " + DateTime.Now + "<p>");
                    return Ok(result);
                }

                return BadRequest("Invalid username or password");
            }

            return BadRequest("Internal Server Error"); //Status code : 400
        }

        // /api/authentication/confirmemail?userId&token
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(EmailConfirmation model)
        {
            if (string.IsNullOrWhiteSpace(model.userId) || string.IsNullOrEmpty(model.token))
            {
                return NotFound();
            }

            var result = await _usersService.ConfirmEmailAsync(model.userId, model.token);

            if (result.IsSuccess)
            {
                //TO DO : Redirect to main page of client
                return Redirect($"{_configuration["AppUrl"]}/ConfirmEmail.html");
            }

            return BadRequest("Internal Server Error"); //Status code : 400
        }

        // api/authentication/forgetpassword
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound();
            }

            var result = await _usersService.ForgetPasswordAsync(email);

            if (result.IsSuccess)
            {
                return Ok(result); //200
            }

            return BadRequest(result); //400
        }

        // api/authentication/resetpassword
        [HttpGet("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordRequest model)
        {
            if (ModelState.IsValid)
            {
                var result = await _usersService.ResetPasswordAsync(model);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Internal Server Error"); //Status code : 400
        }


    }
}
