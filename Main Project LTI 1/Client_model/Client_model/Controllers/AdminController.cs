using Client_model.Models;
using System.Linq;
using System.Web.Mvc;

namespace Client_model.Controllers
{
    public class AdminController : Controller
    {
        private readonly InsuranceDB1Entities _db = new InsuranceDB1Entities();

        // GET: /Admin/Approvals
        public ActionResult Approvals()
        {
            var list = _db.Clients
                .Where(c => c.Status == "Pending")
                .Select(c => new {
                    c.ClientID,
                    c.CompanyName,
                    c.ContactEmail,
                    c.RegisteredAt
                }).ToList();
            return View(list);
        }

        [HttpPost]
        public ActionResult Approve(int id)
        {
            var c = _db.Clients.Find(id);
            if (c != null)
            {
                c.Status = "Approved";
                _db.SaveChanges();
            }
            return RedirectToAction("Approvals");
        }

        [HttpPost]
        public ActionResult Reject(int id)
        {
            var c = _db.Clients.Find(id);
            if (c != null)
            {
                c.Status = "Rejected";
                _db.SaveChanges();
            }
            return RedirectToAction("Approvals");
        }
    }
}
