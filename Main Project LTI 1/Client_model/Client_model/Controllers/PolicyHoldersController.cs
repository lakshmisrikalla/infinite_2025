using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Client_model.Helpers;
using Client_model.Models;
using Client_model.Services;

namespace Client_model.Controllers
{
    public class PolicyHoldersController : Controller
    {
        private readonly UsersService _service;
        private readonly InsuranceDB1Entities _dbContext;

        // Default constructor used by the app
        public PolicyHoldersController()
        {
            // create a DB context and service; keep same pattern as your project
            _dbContext = new InsuranceDB1Entities();
            _service = new UsersService(_dbContext);
        }

        // Optional constructor for unit tests or dependency injection
        public PolicyHoldersController(UsersService service, InsuranceDB1Entities dbContext)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // GET: PolicyHolders
        public ActionResult Index()
        {
            var clientId = ClientContext.CurrentClientId;
            if (clientId == null)
            {
                // not logged in as a client — redirect to login
                return RedirectToAction("Login", "Account");
            }

            IEnumerable<PolicyHolderVm> holders;
            try
            {
                holders = _service.GetPolicyHolders(clientId.Value);
            }
            catch (Exception ex)
            {
                // log error if you have logging; for now show friendly message
                // (replace with your logger, e.g. Log.Error(ex))
                ViewBag.Error = "Unable to load policy holders. " + ex.Message;
                holders = new List<PolicyHolderVm>();
            }

            return View(holders);
        }

        // Optional: Details action to view a single policy holder (by UserPolicyID)
        public ActionResult Details(int id)
        {
            var clientId = ClientContext.CurrentClientId;
            if (clientId == null) return RedirectToAction("Login", "Account");

            // Ensure the requested record belongs to the current client
            var holders = _service.GetPolicyHolders(clientId.Value);
            var vm = System.Linq.Enumerable.FirstOrDefault(holders, h => h.UserPolicyID == id);
            if (vm == null) return HttpNotFound();

            return View(vm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose DB context if we created it here
                _dbContext?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
