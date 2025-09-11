using System.Web.Mvc;
using Client_model.Helpers;
using Client_model.Services;

namespace Client_model.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardService _service;

        public DashboardController()
        {
            _service = new DashboardService(new Models.InsuranceDB1Entities());
        }

        public ActionResult Index()
        {
            var clientId = ClientContext.CurrentClientId;
            if (clientId == null) return RedirectToAction("Login", "Account");

            var summary = _service.GetSummary(clientId.Value);
            return View(summary);
        }
    }
}
