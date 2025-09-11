using System;
using System.Web.Mvc;
using Client_model.Helpers;
using Client_model.Models;
using Client_model.Services;

namespace Client_model.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ReportsService _service;
        private readonly InsuranceDB1Entities _dbContext;

        public ReportsController()
        {
            _dbContext = new InsuranceDB1Entities();
            _service = new ReportsService(_dbContext);
        }

        public ActionResult Index(DateTime? from = null, DateTime? to = null, int? year = null, int? quarter = null)
        {
            var clientId = ClientContext.CurrentClientId;
            if (clientId == null) return RedirectToAction("Login", "Account");

            var vm = _service.GetReportsForClient(clientId.Value, from, to, year, quarter);
            return View(vm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _dbContext?.Dispose();
            base.Dispose(disposing);
        }
    }
}
