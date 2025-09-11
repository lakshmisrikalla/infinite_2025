using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using LTI.Models;
using LTI.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Web;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Security.Cryptography;
using System.Text;
using System.Net;





namespace LTI.Controllers
{
    public class UserController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Register() => View();

        [HttpPost]
        public ActionResult Register(User model, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.FullName) ||
                    string.IsNullOrWhiteSpace(model.Username) ||
                    string.IsNullOrWhiteSpace(model.PhoneNumber) ||
                    string.IsNullOrWhiteSpace(model.Address) ||
                    model.DateOfBirth == default ||
                    string.IsNullOrWhiteSpace(model.Gender) ||
                    string.IsNullOrWhiteSpace(password))
                {
                    ViewBag.Error = "Please fill all required fields.";
                    return View(model);
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 🔍 Check for duplicate email
                    string checkEmailSql = "SELECT COUNT(*) FROM LoginCredentials WHERE Email = @Email";
                    SqlCommand checkCmd = new SqlCommand(checkEmailSql, conn);
                    checkCmd.Parameters.AddWithValue("@Email", model.Email ?? "");
                    int emailCount = (int)checkCmd.ExecuteScalar();

                    if (emailCount > 0)
                    {
                        ViewBag.Error = "Email already registered. Please login or use a different email.";
                        return View(model);
                    }

                    // 🔹 Insert into LoginCredentials
                    string insertLoginSql = @"
                        INSERT INTO LoginCredentials (Username, Email, PasswordHash, Role, IsActive, CreatedAt)
                        VALUES (@Username, @Email, @PasswordHash, 'User', 1, GETUTCDATE());
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand loginCmd = new SqlCommand(insertLoginSql, conn);
                    loginCmd.Parameters.AddWithValue("@Username", model.Username);
                    loginCmd.Parameters.AddWithValue("@Email", (object)model.Email ?? DBNull.Value);
                    loginCmd.Parameters.AddWithValue("@PasswordHash", PasswordHelper.HashPassword(password));

                    int loginID = Convert.ToInt32(loginCmd.ExecuteScalar());

                    // 🔹 Insert into Users (only valid columns)
                    string insertUserSql = @"
                        INSERT INTO Users (LoginID, FullName, PhoneNumber, Address, DateOfBirth, Gender, Status, CreatedAt)
                        VALUES (@LoginID, @FullName, @PhoneNumber, @Address, @DateOfBirth, @Gender, 'Active', GETUTCDATE());
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand userCmd = new SqlCommand(insertUserSql, conn);
                    userCmd.Parameters.AddWithValue("@LoginID", loginID);
                    userCmd.Parameters.AddWithValue("@FullName", model.FullName);
                    userCmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    userCmd.Parameters.AddWithValue("@Address", model.Address);
                    userCmd.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth);
                    userCmd.Parameters.AddWithValue("@Gender", model.Gender);

                    int userID = Convert.ToInt32(userCmd.ExecuteScalar());

                    // 🔹 Set session and redirect
                    Session["UserID"] = userID;
                    Session["FullName"] = model.FullName;
                    return RedirectToAction("Dashboard");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Registration failed: " + (ex.InnerException?.Message ?? ex.Message);
                return View(model);
            }
        }

        public ActionResult Login() => View();

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter both username and password.";
                return View();
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
        SELECT l.LoginID, u.UserID, u.FullName, l.PasswordHash
        FROM LoginCredentials l
        INNER JOIN Users u ON l.LoginID = u.LoginID
        WHERE l.Username = @Username AND l.Role = 'User' AND l.IsActive = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string storedHash = reader["PasswordHash"].ToString();
                    if (PasswordHelper.VerifyPassword(password, storedHash))
                    {
                        Session["UserID"] = reader["UserID"];
                        Session["FullName"] = reader["FullName"].ToString();
                        return RedirectToAction("Overview");
                    }
                }

                ViewBag.Error = "Invalid username or password.";
                return View();
            }
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View("ForgotPassword");
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

        // POST: ForgotPassword
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "❌ Please enter a valid email address.";
                return View("ForgotPassword");
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM LoginCredentials WHERE Email = @Email", con);
                cmd.Parameters.AddWithValue("@Email", email.Trim());
                int count = (int)cmd.ExecuteScalar();
                con.Close();

                if (count == 0)
                {
                    ViewBag.Error = "❌ Email not found. Please register with this email.";
                    return View("ForgotPassword");
                }
            }

            // Generate OTP code
            string code = new Random().Next(100000, 999999).ToString();
            Session["ResetCode"] = code;
            Session["ResetEmail"] = email.Trim();

            // Send email
            string errorMessage;
            bool sent = SendEmail(email.Trim(), "Password Reset Code", $"Your reset code is: {code}", out errorMessage);

            if (!sent)
            {
                ViewBag.Error = "❌ Failed to send email: " + errorMessage;
                return View("ForgotPassword");
            }

            return RedirectToAction("VerifyResetCode");
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

        // GET: VerifyResetCode
        [HttpGet]
        public ActionResult VerifyResetCode()
        {
            return View("VerifyCode"); // Views/User/VerifyCode.cshtml
        }

        [HttpPost]
        public ActionResult VerifyResetCode(string code)
        {
            if (code == Session["ResetCode"]?.ToString())
            {
                return RedirectToAction("ResetPassword");
            }

            ViewBag.Error = "❌ Invalid code.";
            return View("VerifyCode");
        }


        // GET: ResetPassword
        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View("ResetPassword");
        }

        [HttpPost]
        public ActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewBag.Error = "Password fields cannot be empty.";
                return View("ResetPassword");
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "❌ Passwords do not match.";
                return View("ResetPassword");
            }

            if (Session["ResetEmail"] == null)
            {
                ViewBag.Error = "Session expired. Please restart the password reset process.";
                return View("ResetPassword");
            }

            string hashedPassword = PasswordHelper.HashPassword(newPassword.Trim());

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
            UPDATE LoginCredentials 
            SET PasswordHash = @Password 
            WHERE Email = @Email AND Role = 'User' AND IsActive = 1", con);

                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                cmd.Parameters.AddWithValue("@Email", Session["ResetEmail"].ToString());
                cmd.ExecuteNonQuery();
            }

            // Clear reset session
            Session.Remove("ResetEmail");
            Session.Remove("ResetCode");

            TempData["Success"] = "✅ Password reset successful. Please log in with your new password.";
            return RedirectToAction("Login");
        }




        public ActionResult BrowseInsurance(string typeFilter = "All")
        {
            var policies = new List<PolicyViewModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT p.PolicyID, p.PolicyName, p.PlanKind, p.Description, p.DurationMonths, p.BasePremium,
                   pt.TypeName, c.CompanyName
            FROM Policies p
            INNER JOIN PolicyTypes pt ON p.PolicyTypeID = pt.PolicyTypeID
            INNER JOIN Clients c ON p.ClientID = c.ClientID
            WHERE p.Status = 'Approved'";

                if (typeFilter == "Motor" || typeFilter == "Travel")
                {
                    query += " AND pt.TypeName = @TypeName";
                }

                query += " ORDER BY p.CreatedAt DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                if (typeFilter == "Motor" || typeFilter == "Travel")
                {
                    cmd.Parameters.AddWithValue("@TypeName", typeFilter);
                }

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    policies.Add(new PolicyViewModel
                    {
                        PolicyID = Convert.ToInt32(reader["PolicyID"]),
                        PolicyName = reader["PolicyName"].ToString(),
                        PlanKind = reader["PlanKind"].ToString(),
                        Description = reader["Description"].ToString(),
                        DurationMonths = Convert.ToInt32(reader["DurationMonths"]),
                        BasePremium = Convert.ToDecimal(reader["BasePremium"]),
                        TypeName = reader["TypeName"].ToString(),
                        CompanyName = reader["CompanyName"].ToString()
                    });
                }
            }

            ViewBag.TypeFilter = typeFilter;
            return View(policies);
        }

        public ActionResult BuyInsurance(string typeFilter = "All")
        {
            var policies = new List<PolicyViewModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT p.PolicyID, p.PolicyName, p.PlanKind, p.Description, p.DurationMonths, p.BasePremium,
                   pt.TypeName, c.CompanyName
            FROM Policies p
            INNER JOIN PolicyTypes pt ON p.PolicyTypeID = pt.PolicyTypeID
            INNER JOIN Clients c ON p.ClientID = c.ClientID
            WHERE p.Status = 'Approved'";

                if (typeFilter == "Motor" || typeFilter == "Travel")
                {
                    query += " AND pt.TypeName = @TypeName";
                }

                query += " ORDER BY p.CreatedAt DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                if (typeFilter == "Motor" || typeFilter == "Travel")
                {
                    cmd.Parameters.AddWithValue("@TypeName", typeFilter);
                }

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    policies.Add(new PolicyViewModel
                    {
                        PolicyID = Convert.ToInt32(reader["PolicyID"]),
                        PolicyName = reader["PolicyName"].ToString(),
                        PlanKind = reader["PlanKind"].ToString(),
                        Description = reader["Description"].ToString(),
                        DurationMonths = Convert.ToInt32(reader["DurationMonths"]),
                        BasePremium = Convert.ToDecimal(reader["BasePremium"]),
                        TypeName = reader["TypeName"].ToString(),
                        CompanyName = reader["CompanyName"].ToString()
                    });
                }
            }

            ViewBag.TypeFilter = typeFilter;
            return View(policies);
        }


        [HttpPost]
        public ActionResult ConfirmPurchase(int policyId, int emiMonths, int payNowMonths, string paymentMethod, string nomineeName, bool? extendEMI, string UPI)
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            bool isExtended = extendEMI ?? false;

            DateTime startDate = DateTime.Today;
            DateTime endDate = startDate.AddMonths(emiMonths);

            string insuranceType = GetPolicyType(policyId);
            decimal basePremium = GetPolicyPremium(policyId);
            int baseMonths = GetPolicyDuration(policyId);
            decimal monthlyBase = basePremium / baseMonths;

            decimal totalPremium = 0;
            decimal payNowAmount = 0;

            if (isExtended && emiMonths > baseMonths)
            {
                int extraMonths = emiMonths - baseMonths;
                decimal taxedEMI = monthlyBase + (monthlyBase * 0.25m);
                totalPremium = (monthlyBase * baseMonths) + (taxedEMI * extraMonths);

                for (int i = 0; i < payNowMonths; i++)
                {
                    if (i < baseMonths)
                        payNowAmount += monthlyBase;
                    else
                        payNowAmount += taxedEMI;
                }
            }
            else
            {
                totalPremium = basePremium;
                payNowAmount = monthlyBase * payNowMonths;
            }

            int userPolicyId = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 🔹 Insert into UserPolicies
                string insertPolicySql = @"
INSERT INTO UserPolicies (UserID, PolicyID, InsuranceType, StartDate, EndDate, Status, PaymentStatus, CreatedAt, NomineeName)
OUTPUT INSERTED.UserPolicyID
VALUES (@UserID, @PolicyID, @Type, @Start, @End, 'Active', 'Paid', SYSUTCDATETIME(), @Nominee)";


                using (SqlCommand cmd = new SqlCommand(insertPolicySql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@PolicyID", policyId);
                    cmd.Parameters.AddWithValue("@Type", insuranceType);
                    cmd.Parameters.AddWithValue("@Start", startDate);
                    cmd.Parameters.AddWithValue("@End", endDate);
                    cmd.Parameters.AddWithValue("@Nominee", nomineeName ?? "Not Provided");
                    userPolicyId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 🔹 Insert payment for selected months
                string insertPaymentSql = @"
        INSERT INTO Payments (UserPolicyID, Amount, Mode, PaidAt, Status, GatewayRef)
        VALUES (@UserPolicyID, @Amount, @Mode, SYSUTCDATETIME(), 'Success', @Ref)";
                using (SqlCommand payCmd = new SqlCommand(insertPaymentSql, conn))
                {
                    payCmd.Parameters.AddWithValue("@UserPolicyID", userPolicyId);
                    payCmd.Parameters.AddWithValue("@Amount", payNowAmount);
                    payCmd.Parameters.AddWithValue("@Mode", paymentMethod);
                    payCmd.Parameters.AddWithValue("@Ref", "TXN" + Guid.NewGuid().ToString("N").Substring(0, 12));
                    payCmd.ExecuteNonQuery();
                }

                // 🔹 Insert EMI schedule
                for (int i = 0; i < emiMonths; i++)
                {
                    DateTime dueDate = startDate.AddMonths(i);
                    bool isPaid = i < payNowMonths;

                    string insertDueSql = @"
            INSERT INTO PolicyDueDates (UserPolicyID, DueDate, IsPaid, ReminderSent)
            VALUES (@UserPolicyID, @DueDate, @IsPaid, 0)";
                    using (SqlCommand dueCmd = new SqlCommand(insertDueSql, conn))
                    {
                        dueCmd.Parameters.AddWithValue("@UserPolicyID", userPolicyId);
                        dueCmd.Parameters.AddWithValue("@DueDate", dueDate);
                        dueCmd.Parameters.AddWithValue("@IsPaid", isPaid ? 1 : 0);
                        dueCmd.ExecuteNonQuery();
                    }
                }
            }

            // 🔹 Pass data to success view
            ViewBag.PolicyName = GetPolicyName(policyId);
            ViewBag.EMIMonths = emiMonths;
            ViewBag.TotalPremium = totalPremium;
            ViewBag.PaidMonths = payNowMonths;
            ViewBag.PaidAmount = payNowAmount;
            ViewBag.PendingAmount = totalPremium - payNowAmount;

            return View("PurchaseSuccess");
        }


        public ActionResult BuyNow(int id)
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            PolicyViewModel policy = null;
            int userPolicyId = 0;
            bool docsUploaded = false;
            bool docsApproved = false;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 🔹 Get UserPolicyID if it exists
                string getUserPolicySql = @"
            SELECT TOP 1 UserPolicyID FROM UserPolicies
            WHERE UserID = @UserID AND PolicyID = @PolicyID";
                using (SqlCommand cmd = new SqlCommand(getUserPolicySql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@PolicyID", id);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                        userPolicyId = Convert.ToInt32(result);
                }

                // 🔹 Check if documents are uploaded
                if (userPolicyId > 0)
                {
                    string checkDocsSql = @"
                SELECT COUNT(*) FROM Documents
                WHERE OwnerType = 'UserPolicy' AND OwnerID = @UserPolicyID";
                    using (SqlCommand cmd = new SqlCommand(checkDocsSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserPolicyID", userPolicyId);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        docsUploaded = count >= 3; // Expecting Doc1, Doc2, Doc3
                    }

                    // 🔹 Check if documents are approved
                    string checkApprovalSql = @"
                SELECT Decision FROM UserPolicyApprovals
                WHERE UserPolicyID = @UserPolicyID";
                    using (SqlCommand cmd = new SqlCommand(checkApprovalSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserPolicyID", userPolicyId);
                        var decision = cmd.ExecuteScalar();
                        docsApproved = decision != null && decision.ToString() == "Approved";
                    }
                }

                // 🔹 Redirect logic
                if (userPolicyId == 0 || !docsUploaded)
                {
                    TempData["DocRedirectMessage"] = "Please submit required documents before purchasing this policy.";
                    return RedirectToAction("SubmitDocuments", new { policyId = id });
                }

                if (!docsApproved)
                {
                    TempData["DocRedirectMessage"] = "Your documents are under review. You can proceed once they are approved.";
                    return RedirectToAction("SubmitDocuments", new { policyId = id });
                }

                // 🔹 Load policy details
                string query = @"
            SELECT p.PolicyID, p.PolicyName, p.PlanKind, p.Description, p.DurationMonths, p.BasePremium,
                   pt.TypeName, c.CompanyName
            FROM Policies p
            INNER JOIN PolicyTypes pt ON p.PolicyTypeID = pt.PolicyTypeID
            INNER JOIN Clients c ON p.ClientID = c.ClientID
            WHERE p.PolicyID = @PolicyID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PolicyID", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        policy = new PolicyViewModel
                        {
                            PolicyID = Convert.ToInt32(reader["PolicyID"]),
                            PolicyName = reader["PolicyName"].ToString(),
                            PlanKind = reader["PlanKind"].ToString(),
                            Description = reader["Description"].ToString(),
                            DurationMonths = Convert.ToInt32(reader["DurationMonths"]),
                            BasePremium = Convert.ToDecimal(reader["BasePremium"]),
                            TypeName = reader["TypeName"].ToString(),
                            CompanyName = reader["CompanyName"].ToString()
                        };
                    }
                }
            }

            if (policy != null)
                return View(policy);

            return RedirectToAction("Overview");
        }


        private int GetPolicyDuration(int policyId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DurationMonths FROM Policies WHERE PolicyID = @PolicyID", conn);
                cmd.Parameters.AddWithValue("@PolicyID", policyId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }


        // Helper methods
        private string GetPolicyType(int policyId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT pt.TypeName FROM Policies p INNER JOIN PolicyTypes pt ON p.PolicyTypeID = pt.PolicyTypeID WHERE p.PolicyID = @PolicyID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PolicyID", policyId);
                return cmd.ExecuteScalar()?.ToString() ?? "Unknown";
            }
        }

        private decimal GetPolicyPremium(int policyId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT BasePremium FROM Policies WHERE PolicyID = @PolicyID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PolicyID", policyId);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        private string GetPolicyName(int policyId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT PolicyName FROM Policies WHERE PolicyID = @PolicyID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PolicyID", policyId);
                return cmd.ExecuteScalar()?.ToString() ?? "Unknown Policy";
            }
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitDocuments(DocumentSubmissionViewModel model, HttpPostedFileBase Doc1, HttpPostedFileBase Doc2, HttpPostedFileBase Doc3)
        {
            // 1) Session check
            if (Session["UserID"] == null)
            {
                TempData["Error"] = "Session expired. Please log in.";
                return RedirectToAction("Login");
            }

            int userId;
            try { userId = Convert.ToInt32(Session["UserID"]); }
            catch
            {
                TempData["Error"] = "Invalid session user. Please log in again.";
                return RedirectToAction("Login");
            }

            // 2) Defensive: if model is null (form binding failed), build it from Request.Form
            if (model == null)
            {
                model = new DocumentSubmissionViewModel();
                var f = Request.Form;
                int tmpInt;
                DateTime tmpDt;

                if (int.TryParse(f["PolicyID"], out tmpInt)) model.PolicyID = tmpInt;
                if (int.TryParse(f["UserPolicyID"], out tmpInt)) model.UserPolicyID = tmpInt;
                model.PolicyType = f["PolicyType"] ?? "";

                model.ApprovalNotes = f["ApprovalNotes"] ?? "";

                // Travel
                model.PersonName = f["PersonName"] ?? null;
                if (int.TryParse(f["Age"], out tmpInt)) model.Age = tmpInt;
                model.Gender = f["Gender"] ?? null;
                if (DateTime.TryParse(f["DOB"], out tmpDt)) model.DOB = tmpDt;
                model.HealthIssues = f["HealthIssues"] ?? null;

                // Motor
                model.VehicleType = f["VehicleType"] ?? null;
                model.VehicleName = f["VehicleName"] ?? null;
                model.Model = f["Model"] ?? null;
                model.DrivingLicense = f["DrivingLicense"] ?? null;
                model.RegistrationNumber = f["RegistrationNumber"] ?? null;
                model.RCNumber = f["RCNumber"] ?? null;
                if (DateTime.TryParse(f["RegistrationDate"], out tmpDt)) model.RegistrationDate = tmpDt;
                if (DateTime.TryParse(f["ExpiryDate"], out tmpDt)) model.ExpiryDate = tmpDt;
                model.EngineNumber = f["EngineNumber"] ?? null;
                model.ChassisNumber = f["ChassisNumber"] ?? null;
                model.HolderName = f["HolderName"] ?? null;
            }

            // 3) Validate we have either a UserPolicyID or a PolicyID we can use to create one
            if (model.UserPolicyID <= 0 && model.PolicyID <= 0)
            {
                ViewBag.Debug = "SubmitDocuments: both UserPolicyID and PolicyID are missing. Ensure the form posts hidden PolicyID or UserPolicyID.";
                TempData["Warning"] = "Submission failed: missing policy information.";
                return View(model);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // If we don't have a UserPolicy yet, try to find or create one
                    if (model.UserPolicyID <= 0 && model.PolicyID > 0)
                    {
                        string findSql = @"SELECT TOP 1 UserPolicyID FROM UserPolicies WHERE UserID = @UserID AND PolicyID = @PolicyID";
                        using (SqlCommand findCmd = new SqlCommand(findSql, conn))
                        {
                            findCmd.Parameters.AddWithValue("@UserID", userId);
                            findCmd.Parameters.AddWithValue("@PolicyID", model.PolicyID);
                            var found = findCmd.ExecuteScalar();
                            if (found != null && found != DBNull.Value)
                                model.UserPolicyID = Convert.ToInt32(found);
                        }

                        if (model.UserPolicyID <= 0)
                        {
                            string insertUpSql = @"
                        INSERT INTO UserPolicies (UserID, PolicyID, InsuranceType, StartDate, EndDate, Status, PaymentStatus, CreatedAt, NomineeName)
                        OUTPUT INSERTED.UserPolicyID
                        VALUES (@UserID, @PolicyID, @Type, GETDATE(), GETDATE(), 'Pending', 'Unpaid', GETDATE(), 'Pending')";
                            using (SqlCommand insUp = new SqlCommand(insertUpSql, conn))
                            {
                                insUp.Parameters.AddWithValue("@UserID", userId);
                                insUp.Parameters.AddWithValue("@PolicyID", model.PolicyID);
                                insUp.Parameters.AddWithValue("@Type", (object)(model.PolicyType ?? ""));
                                var inserted = insUp.ExecuteScalar();
                                if (inserted != null && inserted != DBNull.Value)
                                    model.UserPolicyID = Convert.ToInt32(inserted);
                            }
                        }
                    }

                    // Final check
                    if (model.UserPolicyID <= 0)
                    {
                        ViewBag.Debug = "SubmitDocuments: Could not resolve or create a UserPolicyID.";
                        TempData["Warning"] = "Submission failed: could not create user policy record.";
                        return View(model);
                    }

                    // 4) Insert Travel or Motor details (safe null handling)
                    if (!string.IsNullOrEmpty(model.PolicyType) && model.PolicyType.Equals("Travel", StringComparison.OrdinalIgnoreCase))
                    {
                        string travelSql = @"
                    INSERT INTO TravelDetails (UserPolicyID, PersonName, Age, Gender, DOB, HealthIssues)
                    VALUES (@UserPolicyID, @Name, @Age, @Gender, @DOB, @Issues)";
                        using (SqlCommand tcmd = new SqlCommand(travelSql, conn))
                        {
                            tcmd.Parameters.AddWithValue("@UserPolicyID", model.UserPolicyID);
                            tcmd.Parameters.AddWithValue("@Name", (object)(model.PersonName ?? ""));
                            tcmd.Parameters.AddWithValue("@Age", model.Age.HasValue ? (object)model.Age.Value : DBNull.Value);
                            tcmd.Parameters.AddWithValue("@Gender", (object)(model.Gender ?? ""));
                            tcmd.Parameters.AddWithValue("@DOB", model.DOB.HasValue ? (object)model.DOB.Value : DBNull.Value);
                            tcmd.Parameters.AddWithValue("@Issues", (object)(model.HealthIssues ?? ""));
                            tcmd.ExecuteNonQuery();
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.PolicyType) && model.PolicyType.Equals("Motor", StringComparison.OrdinalIgnoreCase))
                    {
                        string motorSql = @"
                    INSERT INTO VehicleDetails (UserPolicyID, VehicleType, VehicleName, Model, DrivingLicense, RegistrationNumber, RCNumber, RegistrationDate, ExpiryDate, EngineNumber, ChassisNumber, HolderName)
                    VALUES (@UserPolicyID, @Type, @Name, @Model, @License, @RegNo, @RC, @RegDate, @ExpDate, @Engine, @Chassis, @Holder)";
                        using (SqlCommand mcmd = new SqlCommand(motorSql, conn))
                        {
                            mcmd.Parameters.AddWithValue("@UserPolicyID", model.UserPolicyID);
                            mcmd.Parameters.AddWithValue("@Type", (object)(model.VehicleType ?? ""));
                            mcmd.Parameters.AddWithValue("@Name", (object)(model.VehicleName ?? ""));
                            mcmd.Parameters.AddWithValue("@Model", (object)(model.Model ?? ""));
                            mcmd.Parameters.AddWithValue("@License", (object)(model.DrivingLicense ?? ""));
                            mcmd.Parameters.AddWithValue("@RegNo", (object)(model.RegistrationNumber ?? ""));
                            mcmd.Parameters.AddWithValue("@RC", (object)(model.RCNumber ?? ""));
                            mcmd.Parameters.AddWithValue("@RegDate", model.RegistrationDate.HasValue ? (object)model.RegistrationDate.Value : DBNull.Value);
                            mcmd.Parameters.AddWithValue("@ExpDate", model.ExpiryDate.HasValue ? (object)model.ExpiryDate.Value : DBNull.Value);
                            mcmd.Parameters.AddWithValue("@Engine", (object)(model.EngineNumber ?? ""));
                            mcmd.Parameters.AddWithValue("@Chassis", (object)(model.ChassisNumber ?? ""));
                            mcmd.Parameters.AddWithValue("@Holder", (object)(model.HolderName ?? ""));
                            mcmd.ExecuteNonQuery();
                        }
                    }

                    // 5) Save documents
                    try { SaveDocument(conn, "Doc1", Doc1, model.UserPolicyID); } catch (Exception ex) { throw new Exception("Doc1 save failed: " + ex.Message, ex); }
                    try { SaveDocument(conn, "Doc2", Doc2, model.UserPolicyID); } catch (Exception ex) { throw new Exception("Doc2 save failed: " + ex.Message, ex); }
                    try { SaveDocument(conn, "Doc3", Doc3, model.UserPolicyID); } catch (Exception ex) { throw new Exception("Doc3 save failed: " + ex.Message, ex); }

                    // 6) Create UserPolicyApprovals entry (only if mapping found)
                    try
                    {
                        int clientId = 0;
                        string clientSql = @"
                    SELECT TOP 1 p.ClientID
                    FROM UserPolicies up
                    INNER JOIN Policies p ON up.PolicyID = p.PolicyID
                    WHERE up.UserPolicyID = @UserPolicyID";
                        using (SqlCommand ccmd = new SqlCommand(clientSql, conn))
                        {
                            ccmd.Parameters.AddWithValue("@UserPolicyID", model.UserPolicyID);
                            var cres = ccmd.ExecuteScalar();
                            if (cres != null && cres != DBNull.Value) clientId = Convert.ToInt32(cres);
                        }

                        if (clientId > 0)
                        {
                            string checkSql = "SELECT COUNT(*) FROM UserPolicyApprovals WHERE UserPolicyID = @UserPolicyID";
                            using (SqlCommand chk = new SqlCommand(checkSql, conn))
                            {
                                chk.Parameters.AddWithValue("@UserPolicyID", model.UserPolicyID);
                                int exists = Convert.ToInt32(chk.ExecuteScalar());
                                if (exists == 0)
                                {
                                    string insertApprovalSql = @"
                                INSERT INTO UserPolicyApprovals (UserPolicyID, ClientID, Decision, Notes)
                                VALUES (@UserPolicyID, @ClientID, @Decision, @Notes)";
                                    using (SqlCommand ins = new SqlCommand(insertApprovalSql, conn))
                                    {
                                        ins.Parameters.AddWithValue("@UserPolicyID", model.UserPolicyID);
                                        ins.Parameters.AddWithValue("@ClientID", clientId);
                                        // default to Pending; if your DB CHECK doesn't allow Pending, change to 'Approved' for test
                                        ins.Parameters.AddWithValue("@Decision", "Pending");
                                        ins.Parameters.AddWithValue("@Notes", (object)(model.ApprovalNotes ?? ""));
                                        ins.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    string upd = @"
                                UPDATE UserPolicyApprovals
                                SET DecisionAt = SYSUTCDATETIME(), Notes = ISNULL(Notes,'') + CHAR(13) + @Notes
                                WHERE UserPolicyID = @UserPolicyID";
                                    using (SqlCommand ucmd = new SqlCommand(upd, conn))
                                    {
                                        ucmd.Parameters.AddWithValue("@UserPolicyID", model.UserPolicyID);
                                        ucmd.Parameters.AddWithValue("@Notes", "New docs uploaded on " + DateTime.UtcNow.ToString("u"));
                                        ucmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Not fatal: surface info for debugging
                            ViewBag.Debug = "SubmitDocuments: client mapping not found for UserPolicyID = " + model.UserPolicyID;
                            TempData["Warning"] = "Documents uploaded but approval entry couldn't be created (client mapping missing).";
                        }
                    }
                    catch (SqlException sqlex)
                    {
                        // Common failure: CHECK constraint forbids 'Pending' — show detailed message
                        ViewBag.Debug = $"SQL Error while creating approval: Number={sqlex.Number}, Message={sqlex.Message}";
                        TempData["Warning"] = "Documents uploaded but approval record couldn't be created. See debug info above.";
                        return View(model);
                    }

                    // success
                    TempData["SuccessMessage"] = "Documents and details submitted successfully!";
                    return RedirectToAction("Dashboard");
                } // using conn
            }
            catch (Exception ex)
            {
                // Surface full exception info for debugging (do not keep verbose errors in production)
                ViewBag.Debug = "SubmitDocuments Exception: " + ex.GetType().Name + " - " + ex.Message + (ex.InnerException != null ? (" | Inner: " + ex.InnerException.Message) : "");
                TempData["Warning"] = "Submission failed. See debug info.";
                return View(model);
            }
        }


        public ActionResult SubmitDocuments(int policyId)
        {
            int userId = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0;
            var model = new DocumentSubmissionViewModel
            {
                PolicyID = policyId,
                UserPolicyID = 0,
                PolicyType = "Unknown",
                PolicyName = "Unknown Policy"
            };

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string policySql = @"
            SELECT p.PolicyName, pt.TypeName, up.UserPolicyID
            FROM Policies p
            INNER JOIN PolicyTypes pt ON p.PolicyTypeID = pt.PolicyTypeID
            LEFT JOIN UserPolicies up ON up.PolicyID = p.PolicyID AND up.UserID = @UserID
            WHERE p.PolicyID = @PolicyID";

                using (SqlCommand cmd = new SqlCommand(policySql, conn))
                {
                    cmd.Parameters.AddWithValue("@PolicyID", policyId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.PolicyName = reader["PolicyName"].ToString();
                            model.PolicyType = reader["TypeName"].ToString();
                            model.UserPolicyID = reader["UserPolicyID"] != DBNull.Value ? Convert.ToInt32(reader["UserPolicyID"]) : 0;
                        }
                    }
                }
            }

            return View(model);
        }


        // helper: save uploaded file and insert Documents row
        private void SaveDocument(SqlConnection conn, string docType, HttpPostedFileBase file, int userPolicyId)
        {
            if (file == null || file.ContentLength <= 0) return;

            // ensure Uploads folder exists and file name sanitized
            string uploadsFolder = Server.MapPath("~/Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = Path.GetFileName(file.FileName);
            // Optionally add timestamp to avoid collisions
            string safeName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid().ToString("N").Substring(0, 8)}_{fileName}";
            string fullPath = Path.Combine(uploadsFolder, safeName);

            // Save the file to disk
            file.SaveAs(fullPath);

            // store relative path in DB
            string relativePath = "/Uploads/" + safeName;

            string insertDocSql = @"
        INSERT INTO Documents (OwnerType, OwnerID, DocumentType, FilePath, IsVerified, Visibility)
        VALUES ('UserPolicy', @OwnerID, @Type, @Path, 0, 'ClientAndUser')";

            using (SqlCommand cmd = new SqlCommand(insertDocSql, conn))
            {
                cmd.Parameters.AddWithValue("@OwnerID", userPolicyId);
                cmd.Parameters.AddWithValue("@Type", docType);
                cmd.Parameters.AddWithValue("@Path", relativePath);
                cmd.ExecuteNonQuery();
            }
        }



        public ActionResult ClaimInsurance()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            List<SelectListItem> policies = new List<SelectListItem>();
            Dictionary<int, decimal> policyAmounts = new Dictionary<int, decimal>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
        SELECT up.UserPolicyID, p.PolicyName, pt.TypeName, p.BasePremium
        FROM UserPolicies up
        INNER JOIN Policies p ON up.PolicyID = p.PolicyID
        INNER JOIN PolicyTypes pt ON p.PolicyTypeID = pt.PolicyTypeID
        WHERE up.UserID = @UserID
          AND (up.Status = 'Active' OR up.PaymentStatus = 'Paid')
          AND pt.IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int userPolicyId = Convert.ToInt32(reader["UserPolicyID"]);
                        string label = $"{reader["PolicyName"]} ({reader["TypeName"]})";
                        decimal basePremium = Convert.ToDecimal(reader["BasePremium"]);

                        policies.Add(new SelectListItem
                        {
                            Value = userPolicyId.ToString(),
                            Text = label
                        });

                        policyAmounts[userPolicyId] = basePremium;
                    }
                    reader.Close();
                }
            }

            ViewBag.Policies = policies;
            ViewBag.PolicyAmounts = policyAmounts;
            ViewBag.HasPolicies = policies.Count > 0;

            return View();
        }









        [HttpPost]
        public ActionResult ClaimInsurance(ClaimSubmissionViewModel model, HttpPostedFileBase ClaimDocument)
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            int claimId = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Insert into Claims table
                string claimSql = @"
            INSERT INTO Claims (UserPolicyID, ClaimType, Reason, ClaimedAmount)
            OUTPUT INSERTED.ClaimID
            VALUES (@UserPolicyID, @ClaimType, @Reason, @ClaimedAmount)";
                using (SqlCommand cmd = new SqlCommand(claimSql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserPolicyID", model.UserPolicyID);
                    cmd.Parameters.AddWithValue("@ClaimType", model.ClaimType);
                    cmd.Parameters.AddWithValue("@Reason", model.Reason ?? "");
                    cmd.Parameters.AddWithValue("@ClaimedAmount", model.ClaimedAmount);
                    claimId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Save claim document
                if (ClaimDocument != null && ClaimDocument.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ClaimDocument.FileName);
                    string folderPath = Server.MapPath("~/Uploads");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                    string fullPath = Path.Combine(folderPath, fileName);
                    ClaimDocument.SaveAs(fullPath);

                    string relativePath = "/Uploads/" + fileName;

                    string docSql = @"
                INSERT INTO Documents (OwnerType, OwnerID, DocumentType, FilePath, IsVerified, Visibility)
                VALUES ('UserPolicy', @OwnerID, 'ClaimDocument', @Path, 0, 'ClientAndUser')";
                    using (SqlCommand cmd = new SqlCommand(docSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@OwnerID", model.UserPolicyID);
                        cmd.Parameters.AddWithValue("@Path", relativePath);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            TempData["SuccessMessage"] = "Claim submitted successfully. Sit back and wait — one of our agents will contact you to settle.";
            return View(); // This will render the same ClaimInsurance.cshtml view


        }


        public ActionResult RenewInsurance()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            List<SelectListItem> policies = new List<SelectListItem>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
            SELECT up.UserPolicyID, p.PolicyName, pt.TypeName
            FROM UserPolicies up
            INNER JOIN Policies p ON up.PolicyID = p.PolicyID
            INNER JOIN PolicyTypes pt ON p.PolicyTypeID = pt.PolicyTypeID
            WHERE up.UserID = @UserID
              AND up.PaymentStatus = 'Paid'
              AND up.EndDate <= CAST(SYSUTCDATETIME() AS DATE)
              AND pt.IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        policies.Add(new SelectListItem
                        {
                            Value = reader["UserPolicyID"].ToString(),
                            Text = $"{reader["PolicyName"]} ({reader["TypeName"]})"
                        });
                    }
                }
            }

            ViewBag.Policies = policies;
            return View("RenewInsurance");
        }



        [HttpPost]
        public ActionResult RenewInsurance(int UserPolicyID, decimal Amount)
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Validate policy ownership and eligibility
                string validateSql = @"
            SELECT COUNT(*) FROM UserPolicies
            WHERE UserPolicyID = @UserPolicyID AND UserID = @UserID
              AND PaymentStatus = 'Paid' AND EndDate <= CAST(SYSUTCDATETIME() AS DATE)";
                using (SqlCommand cmd = new SqlCommand(validateSql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserPolicyID", UserPolicyID);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 0)
                    {
                        TempData["ErrorMessage"] = "This policy is not eligible for renewal.";
                        return RedirectToAction("Dashboard");
                    }
                }

                // Insert renewal request
                string insertSql = @"
            INSERT INTO Renewals (UserPolicyID, Amount)
            VALUES (@UserPolicyID, @Amount)";
                using (SqlCommand cmd = new SqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserPolicyID", UserPolicyID);
                    cmd.Parameters.AddWithValue("@Amount", Amount);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["SuccessMessage"] = "Renewal request submitted successfully. Sit back and wait — one of our agents will contact you to settle.";
            return View("RenewInsurance");
        }

        public ActionResult Overview()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login");

            ViewBag.FullName = Session["FullName"]?.ToString();

            var userId = Convert.ToInt32(Session["UserID"]);
            var model = new OverviewViewModel { LatestPolicies = new List<PolicyViewModel>() };

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // ✅ Latest 3 active policies for browsing
                string latestSql = @"
            SELECT TOP 3 PolicyID, PolicyName, Description, BasePremium, DurationMonths
            FROM Policies
            WHERE Status IN ('Draft','Pending','Approved','Rejected','Inactive','Active')
            ORDER BY CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(latestSql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.LatestPolicies.Add(new PolicyViewModel
                        {
                            PolicyID = Convert.ToInt32(reader["PolicyID"]),
                            PolicyName = reader["PolicyName"].ToString(),
                            Description = reader["Description"].ToString(),
                            BasePremium = Convert.ToDecimal(reader["BasePremium"]),
                            DurationMonths = Convert.ToInt32(reader["DurationMonths"])
                        });
                    }
                }

                // ✅ Total policies purchased across platform
                string totalSql = "SELECT COUNT(*) FROM UserPolicies WHERE PaymentStatus = 'Paid'";
                using (SqlCommand cmd = new SqlCommand(totalSql, conn))
                {
                    model.TotalPolicies = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // ✅ Total users with at least one paid policy
                string userSql = @"
            SELECT COUNT(DISTINCT UserID)
            FROM UserPolicies
            WHERE PaymentStatus = 'Paid'";
                using (SqlCommand cmd = new SqlCommand(userSql, conn))
                {
                    model.TotalUsers = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return View(model);
        }


        private PolicyViewModel GetPolicyById(int policyId)
        {
            PolicyViewModel policy = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
        SELECT p.PolicyID, p.PolicyName, pt.TypeName, p.Description,
               p.PlanKind, p.DurationMonths, p.BasePremium
        FROM Policies p
        INNER JOIN PolicyTypes pt ON p.PolicyTypeID = pt.PolicyTypeID
        WHERE p.PolicyID = @PolicyID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PolicyID", policyId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        policy = new PolicyViewModel
                        {
                            PolicyID = Convert.ToInt32(reader["PolicyID"]),
                            PolicyName = reader["PolicyName"].ToString(),
                            TypeName = reader["TypeName"].ToString(),
                            Description = reader["Description"].ToString(),
                            PlanKind = reader["PlanKind"].ToString(),
                            DurationMonths = Convert.ToInt32(reader["DurationMonths"]),
                            BasePremium = Convert.ToDecimal(reader["BasePremium"])
                        };
                    }

                    reader.Close();
                }
            }

            return policy;
        }



        public ActionResult PolicyDetails(int policyId)
        {
            var policy = GetPolicyById(policyId);

            if (policy == null)
            {
                TempData["ErrorMessage"] = "Policy not found.";
                return RedirectToAction("BrowseInsurance");
            }

            return View(policy);
        }




        public ActionResult Dashboard()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login");

            ViewBag.FullName = Session["FullName"]?.ToString();
            return View("Overview");
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        public ActionResult Index() => RedirectToAction("Dashboard");
    }
}