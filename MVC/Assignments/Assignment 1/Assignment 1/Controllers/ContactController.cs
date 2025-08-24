using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Assignment_1.Models;
using Assignment_1.Repositories;


namespace MVC_ContactApp.Controllers
{
        public class ContactController : Controller
        {
            private readonly IContactRepository _repository;

            public ContactController()
            {
                _repository = new ContactRepository();
            }

            // GET: Contact
            public async Task<ActionResult> Index()
            {
                var contacts = await _repository.GetAllAsync();
                return View(contacts);
            }

            // GET: Contact/Create
            public ActionResult Create()
            {
                return View();
            }

            // POST: Contact/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Create(Contact contact)
            {
                if (ModelState.IsValid)
                {
                    await _repository.CreateAsync(contact);
                    return RedirectToAction("Index");
                }
                return View(contact);
            }

            // GET: Contact/Delete
            public async Task<ActionResult> Delete(long id)
            {
                var contact = await _repository.GetByIdAsync(id);
                if (contact == null)
                {
                    return HttpNotFound();
                }
                return View(contact);
            }

            // POST: Contact/Delete
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Delete(Contact contact)
            {
                await _repository.DeleteAsync(contact.Id);
                return RedirectToAction("Index");
            }
        }
    }
