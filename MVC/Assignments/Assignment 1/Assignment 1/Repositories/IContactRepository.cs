using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Assignment_1.Models;

namespace Assignment_1.Repositories
{

        public interface IContactRepository
        {
            Task<List<Contact>> GetAllAsync();
            Task<Contact> GetByIdAsync(long id);
            Task CreateAsync(Contact contact);
            Task DeleteAsync(long id);
        }
  }