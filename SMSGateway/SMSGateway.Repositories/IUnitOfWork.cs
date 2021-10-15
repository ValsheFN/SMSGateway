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
        IContactRepository Contacts { get; }
        Task CommitChangesAsync(string userId);
    }
}
