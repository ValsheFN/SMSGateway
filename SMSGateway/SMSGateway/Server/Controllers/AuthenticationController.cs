using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public AuthenticationController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            var loginResponse = await _usersService.GenerateTokenAsync(model);
            if(loginResponse == null)
            {
                return BadRequest("Invalid username or password");
            }

            return Ok(loginResponse);
        }
    }
}
