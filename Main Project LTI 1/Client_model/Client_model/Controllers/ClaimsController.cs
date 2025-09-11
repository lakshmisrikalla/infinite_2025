using System;
using System.Web.Mvc;
using Client_model.Helpers;
using Client_model.Services;
using Client_model.Models;

namespace Client_model.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ClaimsService _service;
        public ClaimsController()
        {
            _service = new ClaimsService(new InsuranceDB1Entities());
        }

        public ActionResult Index()
        {
            var clientId = ClientContext.CurrentClientId;
            if (clientId == null) return RedirectToAction("Login", "Account");
            var claims = _service.GetClaims(clientId.Value);
            return View(claims);
        }

        [HttpPost]
        public ActionResult Approve(int id, decimal approvedAmount)
        {
            var claim = _service.GetClaimById(id);
            if (claim == null) return HttpNotFound();

            claim.Status = "Approved";
            claim.ApprovedAmount = approvedAmount;
            claim.DecisionAt = DateTime.UtcNow;
            _service.UpdateClaim(claim);
            return RedirectToAction("Index");
        }
    }
}
