using SMSGateway.Server.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway.Repositories
{
    public interface IContactRepository
    {
        Task CreateAsync(Contact contact);
        void Remove(Contact contact);
        IEnumerable<Contact> GetAllAsync(Contact contact);
        Task<Contact> GetByIdAsync(string id);
        Task UpdateAsync(Contact contact);
        List<Contact> GetAllFilteredAsync(string userId);
    }

    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDBContext _db;

        public ContactRepository(ApplicationDBContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(Contact contact)
        {
            await _db.Contact.AddAsync(contact);
        }

        /*public async Task UpdateAsync(Contact contact)
        {
            await _db.Contact.Update(contact);
            await _db.Contact.
        }*/
        IEnumerable<Contact> IContactRepository.GetAllAsync(Contact contact)
        {
            return _db.Contact;
        }

        public async Task<Contact> GetByIdAsync(string id)
        {
            return await _db.Contact.FindAsync(id);
        }

        public void Remove(Contact contact)
        {
            _db.Contact.Remove(contact);
        }

        public Task UpdateAsync(Contact contact)
        {
            throw new NotImplementedException();
        }

        public List<Contact> GetAllFilteredAsync(string userId)
        {
            return _db.Contact.Where(x => x.CreatedByUserId == userId).ToList();
        }
    }
}
