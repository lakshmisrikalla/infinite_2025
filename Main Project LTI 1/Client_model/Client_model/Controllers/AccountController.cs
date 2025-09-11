using System;
using System.Linq;
using System.Web.Mvc;
using Client_model.Helpers;
using Client_model.Models;

namespace Client_model.Controllers
{
    public class AccountController : Controller
    {
        private readonly InsuranceDB1Entities _db;

        // Default constructor for normal usage
        public AccountController() : this(new InsuranceDB1Entities())
        {
        }

        // DI constructor for unit testing
        public AccountController(InsuranceDB1Entities context)
        {
            _db = context;
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            return View(new RegisterClientVm());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterClientVm model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_db.LoginCredentials.Any(x => x.Username == model.Username))
            {
                ModelState.AddModelError(nameof(model.Username), "Username already taken");
                return View(model);
            }
            if (_db.LoginCredentials.Any(x => x.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email already registered");
                return View(model);
            }
            if (_db.Clients.Any(c => c.CompanyName == model.CompanyName))
            {
                ModelState.AddModelError(nameof(model.CompanyName), "Company name already exists");
                return View(model);
            }

            var login = new LoginCredential
            {
                Email = model.Email,
                Username = model.Username,
                PasswordHash = HashHelper.HashPassword(model.Password),
                Role = "Client",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _db.LoginCredentials.Add(login);
            _db.SaveChanges();

            string companyCode;
            do
            {
                companyCode = HashHelper.GenerateCompanyCode(8);
            } while (_db.Clients.Any(c => c.CompanyCode == companyCode));

            var client = new Client
            {
                LoginID = login.LoginID,
                CompanyName = model.CompanyName,
                CompanyCode = companyCode,
                ContactEmail = model.Email,
                ContactPhone = model.ContactPhone,
                Status = "Pending",
                RegisteredAt = DateTime.UtcNow
            };
            _db.Clients.Add(client);
            _db.SaveChanges();

            var body = $@"
                <p>Hi {model.CompanyName},</p>
                <p>Thanks for registering to LTI Insurance. Your Company Code is <strong>{companyCode}</strong>.</p>
                <p>Use <strong>Username</strong>, <strong>Password</strong> and this <strong>Company Code</strong> to login.</p>
                <p>— LTI Insurance</p>";

            try
            {
                EmailHelper.SendEmail(model.Email, "Your LTI Insurance Company Code", body);
                TempData["Success"] = "Registration successful. Company code emailed. Await admin approval.";
            }
            catch (Exception)
            {
                TempData["Success"] = $"Registered. (Email not sent) Company code: {companyCode}. Await admin approval.";
            }

            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        public ActionResult Login()
        {
            return View(new LoginVm());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginVm model)
        {
            if (!ModelState.IsValid) return View(model);

            var login = _db.LoginCredentials.FirstOrDefault(l => l.Username == model.Username && l.Role == "Client");
            if (login == null || !HashHelper.VerifyPassword(model.Password, login.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            var client = _db.Clients.FirstOrDefault(c => c.LoginID == login.LoginID && c.CompanyCode == model.CompanyCode);
            if (client == null)
            {
                ModelState.AddModelError("", "Invalid company code for this user.");
                return View(model);
            }

            if (client.Status != "Approved")
            {
                ModelState.AddModelError("", $"Your client account is '{client.Status}'. Please wait for admin approval.");
                return View(model);
            }

            Session["LoginID"] = login.LoginID;
            Session["ClientID"] = client.ClientID;
            Session["Username"] = login.Username;
            return RedirectToAction("MyProfile", "Client");
        }

        // GET: /Account/Forgot
        public ActionResult Forgot()
        {
            return View(new ForgotVm());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Forgot(ForgotVm model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _db.Clients.FirstOrDefault(c => c.ContactEmail == model.Email && c.CompanyName == model.CompanyName);
            if (client == null)
            {
                ModelState.AddModelError("", "No client found with this email and company name.");
                return View(model);
            }

            var login = _db.LoginCredentials.FirstOrDefault(l => l.LoginID == client.LoginID);
            if (login == null)
            {
                ModelState.AddModelError("", "Account credentials missing.");
                return View(model);
            }

            var temp = HashHelper.GenerateTempPassword(10);
            login.PasswordHash = HashHelper.HashPassword(temp);
            _db.SaveChanges();

            var body = $@"
                <p>Hi {client.CompanyName},</p>
                <p>Your temporary password is: <strong>{temp}</strong></p>
                <p>Please login and change it immediately. Company Code: <strong>{client.CompanyCode}</strong></p>
                <p>- LTI Insurance</p>";

            try
            {
                EmailHelper.SendEmail(model.Email, "LTI Insurance - Temporary Password", body);
                TempData["Success"] = "Temporary password sent to your email.";
            }
            catch (Exception)
            {
                TempData["Success"] = $"Temporary password generated (email failed): {temp} — Company Code: {client.CompanyCode}";
            }

            return RedirectToAction("Login");
        }

        // GET: Change Password (must be logged in)
        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (Session["LoginID"] == null) return RedirectToAction("Login");
            return View(new ChangePasswordVm());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordVm model)
        {
            if (!ModelState.IsValid) return View(model);
            if (Session["LoginID"] == null) return RedirectToAction("Login");

            int loginId = (int)Session["LoginID"];
            var login = _db.LoginCredentials.Find(loginId);

            if (login == null || !HashHelper.VerifyPassword(model.CurrentPassword, login.PasswordHash))
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View(model);
            }

            login.PasswordHash = HashHelper.HashPassword(model.NewPassword);
            _db.SaveChanges();

            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("MyProfile", "Client");
        }

        // GET: Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
