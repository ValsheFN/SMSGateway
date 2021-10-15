using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SMSGateway.Server.Services;
using SMSGateway.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSGateway.Repositories;

namespace SMSGateway.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private IUsersService _usersService;
        private IMailService _mailService;
        private IConfiguration _configuration;
        private readonly IContactRepository _contactRepository;

        public ContactController(IUsersService usersService, IMailService mailService, IConfiguration configuration,
                                IContactRepository contactRepository)
        {
            _usersService = usersService;
            _mailService = mailService;
            _configuration = configuration;
            _contactRepository = contactRepository;
        }

        // /api/authentication/register
        [HttpGet("ContactFiltered")]
        public async Task<IActionResult> GetContactFiltered(string userId)
        {
            return Ok(_contactRepository.GetAllFilteredAsync(userId));
        }
    }
}
