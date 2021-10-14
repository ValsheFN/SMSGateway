using Microsoft.AspNetCore.Identity;
using SMSGateway.Server.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway.Repositories
{
    public interface IUnitOfWork
    {
        IUsersRepository Users { get; }

        Task CommitChangesAsync();
    }

    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDBContext _db;

        public EfUnitOfWork(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
                            ApplicationDBContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        private IUsersRepository _users;
        public IUsersRepository Users
        {
            get
            {
                if(_users == null)
                {
                    _users = new IdentityUsersRepository(_userManager, _roleManager);
                }

                return _users;
            }
        }

        public async Task CommitChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
