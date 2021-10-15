using Microsoft.AspNetCore.Identity;
using SMSGateway.Repositories;
using SMSGateway.Server.Infrastructure;
using SMSGateway.Server.Models.Models;
using SMSGateway.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway.Server.Services
{
    public interface IContactServices
    {
        Task<OperationResponse<ContactDetail>> CreateAsync(ContactDetail model);
        Task<OperationResponse<ContactDetail>> UpdateAsync(ContactDetail model);
        Task<OperationResponse<ContactDetail>> RemoveAsync(string id);
        //Task<OperationResponse<ContactDetail>> CreateAsync(ContactDetail model);
        List<Contact> GetFilteredAsync(string userId);
    }

    public class ContactServices : IContactServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EfUnitOfWork _efUnitOfWork;
        private readonly IdentityOption _identity;

        public ContactServices(IUnitOfWork unitOfWork,
                                IdentityOption identity,
                                EfUnitOfWork efUnitOfWork)
        {
            _unitOfWork = unitOfWork;
            _efUnitOfWork = efUnitOfWork;
            _identity = identity;
        }

        public async Task<OperationResponse<ContactDetail>> CreateAsync(ContactDetail model)
        {
            var contact = new Contact
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Notes = model.Notes
            };

            await _unitOfWork.Contacts.CreateAsync(contact);
            await _unitOfWork.CommitChangesAsync(_identity.UserId);

            model.Id = contact.Id;

            return new OperationResponse<ContactDetail>
            {
                IsSuccess = true,
                Message = "Contact created successfully!",
                Data = model
            };
        }

        public List<Contact> GetFilteredAsync(string userId)
        {
            await _unitOfWork.Contacts.GetAllFilteredAsync(userId);
        }

        public Task<OperationResponse<ContactDetail>> RemoveAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponse<ContactDetail>> UpdateAsync(ContactDetail model)
        {
            throw new NotImplementedException();
        }
    }
}
