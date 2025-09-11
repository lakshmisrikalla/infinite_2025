using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using LTI.Models;
using System.Net;
using System.Net.Mail;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace LTI.Controllers
{
    public class AccountController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        // 🔐 Admin Registration
        [HttpGet]
        public ActionResult RegisterAdmin()
        {
            ViewBag.Captcha = GenerateCaptcha();
            return View(new AdminRegistrationViewModel());
        }

        [HttpPost]
        public ActionResult RegisterAdmin(AdminRegistrationViewModel model)
        {
            // 🔐 Captcha Validation
            if (model.Captcha != Session["Captcha"]?.ToString())
            {
                ModelState.AddModelError("Captcha", "Invalid captcha.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Captcha = GenerateCaptcha();
                return View(model);
            }

            try
            {
                con.Open();

                // 🧠 Check for duplicate Email
                SqlCommand checkEmailCmd = new SqlCommand("SELECT COUNT(*) FROM LoginCredentials WHERE Email = @Email", con);
                checkEmailCmd.Parameters.AddWithValue("@Email", model.Email.Trim());
                int emailCount = (int)checkEmailCmd.ExecuteScalar();

                if (emailCount > 0)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                }

                // 🧠 Check for duplicate Username
                SqlCommand checkUsernameCmd = new SqlCommand("SELECT COUNT(*) FROM LoginCredentials WHERE Username = @Username", con);
                checkUsernameCmd.Parameters.AddWithValue("@Username", model.Username.Trim());
                int usernameCount = (int)checkUsernameCmd.ExecuteScalar();

                if (usernameCount > 0)
                {
                    ModelState.AddModelError("Username", "This username is already taken.");
                }

                // 🧠 Check for duplicate Contact Number
                SqlCommand checkContactCmd = new SqlCommand("SELECT COUNT(*) FROM Admins WHERE ContactNumber = @ContactNumber", con);
                checkContactCmd.Parameters.AddWithValue("@ContactNumber", model.ContactNumber.Trim());
                int contactCount = (int)checkContactCmd.ExecuteScalar();

                if (contactCount > 0)
                {
                    ModelState.AddModelError("ContactNumber", "This contact number is already registered.");
                }

                // 🚫 If any duplicates found, return with errors
                if (!ModelState.IsValid)
                {
                    ViewBag.Captcha = GenerateCaptcha();
                    return View(model);
                }

                // 🔐 Hash password
                string hashedPassword = HashPassword(model.Password.Trim());

                // ✅ Insert into LoginCredentials
                SqlCommand cmd1 = new SqlCommand(@"
            INSERT INTO LoginCredentials 
            (Email, Username, PasswordHash, Role, IsActive) 
            OUTPUT INSERTED.LoginID 
            VALUES (@Email, @Username, @PasswordHash, 'Admin', 1)", con);

                cmd1.Parameters.AddWithValue("@Email", model.Email.Trim());
                cmd1.Parameters.AddWithValue("@Username", model.Username.Trim());
                cmd1.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                int loginId = (int)cmd1.ExecuteScalar();

                // ✅ Insert into Admins
                SqlCommand cmd2 = new SqlCommand(@"
            INSERT INTO Admins 
            (LoginID, FullName, ContactNumber) 
            VALUES (@LoginID, @FullName, @ContactNumber)", con);

                cmd2.Parameters.AddWithValue("@LoginID", loginId);
                cmd2.Parameters.AddWithValue("@FullName", model.Name.Trim());
                cmd2.Parameters.AddWithValue("@ContactNumber", model.ContactNumber.Trim());

                cmd2.ExecuteNonQuery();

                // 🎉 Redirect to Login with success message
                TempData["Success"] = "✅ Registration successful. Please log in.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong during registration. Please try again.");
                ViewBag.Captcha = GenerateCaptcha();
                return View(model);
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // 🔐 Admin Login
        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.Captcha = GenerateCaptcha();
            return View();
        }

        [HttpPost]
        public ActionResult Login(string UsernameOrEmail, string Password, string Captcha)
        {
            string expectedCaptcha = Session["Captcha"]?.ToString();
            if (expectedCaptcha == null || Captcha != expectedCaptcha)
            {
                TempData["Error"] = "❌ Invalid captcha. Please try again.";
                return RedirectToAction("Login");
            }

            string hashedInput = HashPassword(Password.Trim());
            bool isValidAdmin = false;
            int loginId = 0;
            string adminName = "";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string query = @"
                    SELECT LoginID, Username FROM LoginCredentials
                    WHERE (Username = @Input OR Email = @Input)
                    AND PasswordHash = @Password
                    AND Role = 'Admin'
                    AND IsActive = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Input", UsernameOrEmail.Trim());
                cmd.Parameters.AddWithValue("@Password", hashedInput);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    isValidAdmin = true;
                    loginId = Convert.ToInt32(reader["LoginID"]);
                    adminName = reader["Username"].ToString();
                }
                conn.Close();
            }

            if (!isValidAdmin)
            {
                TempData["Error"] = "❌ Invalid credentials or not an active admin.";
                return RedirectToAction("Login");
            }

            Session["LoginID"] = loginId;
            Session["AdminName"] = adminName;
            Session["AdminLoggedIn"] = true;

            return RedirectToAction("Dashboard", "Admin");
        }

        // 🔧 Helpers
        private string GenerateCaptcha()
        {
            string captcha = new Random().Next(1000, 9999).ToString();
            Session["Captcha"] = captcha;
            return captcha;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        public ActionResult Logout()
        {
            Session.Clear(); // Clears all session variables
            TempData["Success"] = "✅ Logged out successfully.";
            return RedirectToAction("Login", "Account");
        }

        public ActionResult RegisterOptions() => View();

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: ForgotPassword
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM LoginCredentials WHERE Email = @Email", con);
            cmd.Parameters.AddWithValue("@Email", email.Trim());
            int count = (int)cmd.ExecuteScalar();
            con.Close();

            if (count == 0)
            {
                ViewBag.Error = "❌ Email not found.Please register with this email";
                return View();
            }

            string code = new Random().Next(100000, 999999).ToString();
            Session["ResetCode"] = code;
            Session["ResetEmail"] = email.Trim();

            string errorMessage;
            bool sent = SendEmail(email.Trim(), "Password Reset Code", $"Your reset code is: {code}", out errorMessage);

            if (!sent)
            {
                ViewBag.Error = "❌ Failed to send email: " + errorMessage;
                return View();
            }

            return RedirectToAction("VerifyResetCode");
        }

        // GET: VerifyResetCode
        [HttpGet]
        public ActionResult VerifyResetCode()
        {
            return View();
        }

        // POST: VerifyResetCode
        [HttpPost]
        public ActionResult VerifyResetCode(string code)
        {
            if (code == Session["ResetCode"]?.ToString())
            {
                return RedirectToAction("ResetPassword");
            }

            ViewBag.Error = "❌ Invalid code.";
            return View();
        }

        // GET: ResetPassword
        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        // POST: ResetPassword
        [HttpPost]
        public ActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "❌ Passwords do not match.";
                return View();
            }

            string hashed = HashPassword(newPassword.Trim());

            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE LoginCredentials SET PasswordHash = @Password WHERE Email = @Email", con);
            cmd.Parameters.AddWithValue("@Password", hashed);
            cmd.Parameters.AddWithValue("@Email", Session["ResetEmail"].ToString());
            cmd.ExecuteNonQuery();
            con.Close();

            Session.Remove("ResetCode");
            Session.Remove("ResetEmail");

            TempData["Success"] = "✅ Password reset successful. Please log in.";
            return RedirectToAction("Login");
        }

        private bool SendEmail(string toEmail, string subject, string body, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    ConfigurationManager.AppSettings["FromName"],
                    ConfigurationManager.AppSettings["FromEmail"]
                ));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;
                email.Body = new TextPart("plain") { Text = body };

                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    smtp.Connect(
                        ConfigurationManager.AppSettings["SmtpHost"],
                        int.Parse(ConfigurationManager.AppSettings["SmtpPort"]),
                        SecureSocketOptions.StartTls
                    );

                    smtp.Authenticate(
                        ConfigurationManager.AppSettings["SmtpUser"],
                        ConfigurationManager.AppSettings["SmtpPass"]
                    );

                    smtp.Send(email);
                    smtp.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

    }
}
