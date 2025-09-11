using System.Linq;
using System.Web.Mvc;
using Client_model.Helpers;
using Client_model.Models;

namespace Client_model.Controllers
{
    public class RenewalsController : Controller
    {
        private readonly InsuranceDB1Entities _db = new InsuranceDB1Entities();

        public ActionResult Index()
        {
            var list = _db.Renewals
                .Where(r => r.UserPolicy.Policy.ClientID == ClientContext.CurrentClientId)
                .OrderByDescending(r => r.RequestedAt)
                .Select(r => new
                {
                    Policy = r.UserPolicy.Policy.PolicyName,
                    Holder = r.UserPolicy.User.FullName,
                    r.Amount,
                    r.Status,
                    r.RequestedAt,
                    r.ApprovedAt
                }).ToList();

            return View(list);
        }
    }
}
