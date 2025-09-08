using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using LTI.Models;
using LTI.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Web;




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
                cmd.Parameters.AddWithValue("@Username", username); // ✅ This line is essential

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string storedHash = reader["PasswordHash"].ToString();
                    if (PasswordHelper.VerifyPassword(password, storedHash))
                    {
                        Session["UserID"] = reader["UserID"];
                        Session["FullName"] = reader["FullName"].ToString();
                        return RedirectToAction("Dashboard");
                    }
                }

                ViewBag.Error = "Invalid username or password.";
                return View();
            }
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

            return RedirectToAction("Dashboard");
        }


        [HttpPost]
        public ActionResult SubmitDocuments(DocumentSubmissionViewModel model, HttpPostedFileBase Doc1, HttpPostedFileBase Doc2, HttpPostedFileBase Doc3)
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 🔹 Insert Travel or Motor details
                if (model.PolicyType == "Travel")
                {
                    string travelSql = @"
                INSERT INTO TravelDetails (UserPolicyID, PersonName, Age, Gender, DOB, HealthIssues)
                VALUES (@UserPolicyID, @Name, @Age, @Gender, @DOB, @Issues)";
                    using (SqlCommand cmd = new SqlCommand(travelSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserPolicyID", model.UserPolicyID);
                        cmd.Parameters.AddWithValue("@Name", model.PersonName);
                        cmd.Parameters.AddWithValue("@Age", model.Age);
                        cmd.Parameters.AddWithValue("@Gender", model.Gender);
                        cmd.Parameters.AddWithValue("@DOB", model.DOB);
                        cmd.Parameters.AddWithValue("@Issues", model.HealthIssues ?? "");
                        cmd.ExecuteNonQuery();
                    }
                }
                else if (model.PolicyType == "Motor")
                {
                    string motorSql = @"
                INSERT INTO VehicleDetails (UserPolicyID, VehicleType, VehicleName, Model, DrivingLicense, RegistrationNumber, RCNumber, RegistrationDate, ExpiryDate, EngineNumber, ChassisNumber, HolderName)
                VALUES (@UserPolicyID, @Type, @Name, @Model, @License, @RegNo, @RC, @RegDate, @ExpDate, @Engine, @Chassis, @Holder)";
                    using (SqlCommand cmd = new SqlCommand(motorSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserPolicyID", model.UserPolicyID);
                        cmd.Parameters.AddWithValue("@Type", model.VehicleType);
                        cmd.Parameters.AddWithValue("@Name", model.VehicleName);
                        cmd.Parameters.AddWithValue("@Model", model.Model);
                        cmd.Parameters.AddWithValue("@License", model.DrivingLicense);
                        cmd.Parameters.AddWithValue("@RegNo", model.RegistrationNumber);
                        cmd.Parameters.AddWithValue("@RC", model.RCNumber);
                        cmd.Parameters.AddWithValue("@RegDate", model.RegistrationDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ExpDate", model.ExpiryDate);
                        cmd.Parameters.AddWithValue("@Engine", model.EngineNumber);
                        cmd.Parameters.AddWithValue("@Chassis", model.ChassisNumber);
                        cmd.Parameters.AddWithValue("@Holder", model.HolderName);
                        cmd.ExecuteNonQuery();
                    }
                }

                // 🔹 Upload documents
                SaveDocument(conn, "Doc1", Doc1, model.UserPolicyID);
                SaveDocument(conn, "Doc2", Doc2, model.UserPolicyID);
                SaveDocument(conn, "Doc3", Doc3, model.UserPolicyID);
            }

            TempData["SuccessMessage"] = "Documents and details submitted successfully!";
            return RedirectToAction("Dashboard");
        }

        private void SaveDocument(SqlConnection conn, string docType, HttpPostedFileBase file, int userPolicyId)
        {
            if (file != null && file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string folderPath = Server.MapPath("~/Uploads");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fullPath = Path.Combine(folderPath, fileName);
                file.SaveAs(fullPath);

                string relativePath = "/Uploads/" + fileName;

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
        }


        public ActionResult SubmitDocuments(int policyId)
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            DocumentSubmissionViewModel model = new DocumentSubmissionViewModel();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Get policy info and type
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
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        model.PolicyID = policyId;
                        model.PolicyName = reader["PolicyName"].ToString();
                        model.PolicyType = reader["TypeName"].ToString(); // Travel or Motor
                        model.UserPolicyID = reader["UserPolicyID"] != DBNull.Value
                            ? Convert.ToInt32(reader["UserPolicyID"])
                            : 0;
                    }
                }

                // Create UserPolicy if not exists
                if (model.UserPolicyID == 0)
                {
                    string insertSql = @"
                INSERT INTO UserPolicies (UserID, PolicyID, InsuranceType, StartDate, EndDate, Status, PaymentStatus, CreatedAt, NomineeName)
                OUTPUT INSERTED.UserPolicyID
                VALUES (@UserID, @PolicyID, @Type, GETDATE(), GETDATE(), 'Pending', 'Unpaid', GETDATE(), 'Pending')";
                    using (SqlCommand cmd = new SqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@PolicyID", policyId);
                        cmd.Parameters.AddWithValue("@Type", model.PolicyType);
                        model.UserPolicyID = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }

            return View(model);
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
                }
            }

            if (policies.Count == 0)
            {
                TempData["ErrorMessage"] = "You have no active or paid policies eligible for claims.";
                return RedirectToAction("Dashboard");
            }

            ViewBag.Policies = policies;
            ViewBag.PolicyAmounts = policyAmounts;
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
            return View("ClaimInsurance");
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




        public ActionResult Dashboard()
        {
            ViewBag.FullName = Session["FullName"]?.ToString();
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        public ActionResult Index() => RedirectToAction("Dashboard");
    }
}