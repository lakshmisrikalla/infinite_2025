using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Client_model.Helpers;
using Client_model.Models;
using Client_model.Services;

namespace Client_model.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly PaymentsService _service;
        private readonly InsuranceDB1Entities _dbContext;

        public PaymentsController()
        {
            _dbContext = new InsuranceDB1Entities();
            _service = new PaymentsService(_dbContext);
        }

        // DI-friendly constructor (optional)
        public PaymentsController(PaymentsService service, InsuranceDB1Entities dbContext)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // GET: Payments
        public ActionResult Index()
        {
            var clientId = ClientContext.CurrentClientId;
            if (clientId == null) return RedirectToAction("Login", "Account");

            IEnumerable<PaymentVm> model;
            try
            {
                model = _service.GetPaymentsForClient(clientId.Value);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load payments: " + ex.Message;
                model = new List<PaymentVm>();
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
