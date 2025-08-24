using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;
using Assignment_1.Models;


namespace Assignment_1.Repositories
{
  
        public class ContactRepository : IContactRepository
        {
            private readonly ContactContext _context;

            public ContactRepository()
            {
                _context = new ContactContext();
            }

            public async Task<List<Contact>> GetAllAsync()
            {
                return await _context.Contacts.ToListAsync();
            }

            public async Task<Contact> GetByIdAsync(long id)
            {
                return await _context.Contacts.FindAsync(id);
            }

            public async Task CreateAsync(Contact contact)
            {
                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteAsync(long id)
            {
                var contact = await _context.Contacts.FindAsync(id);
                if (contact != null)
                {
                    _context.Contacts.Remove(contact);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
