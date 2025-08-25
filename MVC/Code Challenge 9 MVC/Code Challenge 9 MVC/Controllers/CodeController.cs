using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Code_Challenge_9_MVC.Controllers
{
    public class CodeController : Controller
    {
        private NorthWindEntities db = new NorthWindEntities();

        //  Customers from Germany
        public ActionResult CustomersInGermany()
        {
            var germanCustomers = db.Customers
                .Where(c => c.Country == "Germany")
                .ToList();

            return View(germanCustomers);
        }

        // customer with OrderId == 10248
        public ActionResult CustomerByOrder()
        {
            var customer = db.Orders
                .Where(o => o.OrderID == 10248)
                .Select(o => o.Customer)
                .FirstOrDefault();

            return View(customer);
        }
    }
}
