using System.Linq;
using System.Web.Mvc;
using Client_model.Helpers;
using Client_model.Models;

namespace Client_model.Controllers
{
    public class DueDatesController : Controller
    {
        private readonly InsuranceDB1Entities _db = new InsuranceDB1Entities();

        public ActionResult Index()
        {
            var list = _db.PolicyDueDates
                .Where(d => d.UserPolicy.Policy.ClientID == ClientContext.CurrentClientId)
                .OrderBy(d => d.DueDate)
                .Select(d => new
                {
                    Policy = d.UserPolicy.Policy.PolicyName,
                    Holder = d.UserPolicy.User.FullName,
                    d.DueDate,
                    d.IsPaid,
                    d.ReminderSent
                }).ToList();

            return View(list);
        }
    }
}
