using System.Linq;
using System.Web.Mvc;
using Client_model.Models;

namespace Client_model.Controllers
{
    public class ClientController : Controller
    {
        private readonly InsuranceDB1Entities _db = new InsuranceDB1Entities();

        // Legacy redirect to keep older links working
        public new ActionResult Profile()
        {
            return RedirectToAction("MyProfile");
        }

        // GET: /Client/MyProfile
        public ActionResult MyProfile()
        {
            var clientId = Helpers.ClientContext.CurrentClientId;
            if (clientId == null) return RedirectToAction("Login", "Account");

            var client = _db.Clients.FirstOrDefault(c => c.ClientID == clientId.Value);
            if (client == null) return HttpNotFound();

            var vm = new ProfileEditVm
            {
                CompanyName = client.CompanyName,
                ContactEmail = client.ContactEmail,
                ContactPhone = client.ContactPhone
            };

            // counts for display
            ViewBag.PoliciesCount = _db.Policies.Count(p => p.ClientID == clientId.Value);
            ViewBag.ActivePoliciesCount = _db.Policies.Count(p => p.ClientID == clientId.Value && p.IsActive);
            ViewBag.HoldersCount = _db.UserPolicies.Where(up => up.Policy.ClientID == clientId.Value).Select(up => up.UserID).Distinct().Count();

            // new: show registered date and company code (for display only)
            ViewBag.RegisteredAt = client.RegisteredAt;
            ViewBag.CompanyCode = client.CompanyCode;

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MyProfile(ProfileEditVm model)
        {
            var clientId = Helpers.ClientContext.CurrentClientId;
            if (clientId == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                // refill counts & details for view
                ViewBag.PoliciesCount = _db.Policies.Count(p => p.ClientID == clientId.Value);
                ViewBag.ActivePoliciesCount = _db.Policies.Count(p => p.ClientID == clientId.Value && p.Status == "Active");
                ViewBag.HoldersCount = _db.UserPolicies.Where(up => up.Policy.ClientID == clientId.Value).Select(up => up.UserID).Distinct().Count();

                var existing = _db.Clients.FirstOrDefault(c => c.ClientID == clientId.Value);
                ViewBag.RegisteredAt = existing?.RegisteredAt;
                ViewBag.CompanyCode = existing?.CompanyCode;

                return View(model);
            }

            var client = _db.Clients.FirstOrDefault(c => c.ClientID == clientId.Value);
            if (client == null) return HttpNotFound();

            // Update client fields
            client.CompanyName = model.CompanyName;
            client.ContactEmail = model.ContactEmail;
            client.ContactPhone = model.ContactPhone;

            // Keep LoginCredentials.Email in sync
            var login = _db.LoginCredentials.Find(client.LoginID);
            if (login != null && login.Email != model.ContactEmail)
            {
                if (_db.LoginCredentials.Any(x => x.Email == model.ContactEmail && x.LoginID != login.LoginID))
                {
                    ModelState.AddModelError("ContactEmail", "Email already in use by another account.");
                    // refill counts & details for view
                    ViewBag.PoliciesCount = _db.Policies.Count(p => p.ClientID == clientId.Value);
                    ViewBag.ActivePoliciesCount = _db.Policies.Count(p => p.ClientID == clientId.Value && p.Status == "Active");
                    ViewBag.HoldersCount = _db.UserPolicies.Where(up => up.Policy.ClientID == clientId.Value).Select(up => up.UserID).Distinct().Count();

                    ViewBag.RegisteredAt = client.RegisteredAt;
                    ViewBag.CompanyCode = client.CompanyCode;
                    return View(model);
                }
                login.Email = model.ContactEmail;
            }

            _db.SaveChanges();
            TempData["Success"] = "Profile updated.";
            return RedirectToAction("MyProfile");
        }
    }
}
