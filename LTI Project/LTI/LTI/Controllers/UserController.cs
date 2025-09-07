using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using LTI.Models;
using LTI.Helpers;
using System.Collections.Generic;


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
        public ActionResult ConfirmPurchase(int policyId, int emiMonths, string paymentMethod, string nomineeName, bool extendEMI)
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            DateTime startDate = DateTime.Today;
            DateTime endDate = startDate.AddMonths(emiMonths);
            string insuranceType = GetPolicyType(policyId); // helper method below

            decimal basePremium = GetPolicyPremium(policyId);
            decimal totalPremium = extendEMI ? basePremium * 1.35m : basePremium;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Insert into UserPolicies
                string insertPolicySql = @"
            INSERT INTO UserPolicies (UserID, PolicyID, InsuranceType, StartDate, EndDate, Status, PaymentStatus, CreatedAt, NomineeName)
            VALUES (@UserID, @PolicyID, @Type, @Start, @End, 'Active', 'Paid', SYSUTCDATETIME(), @Nominee)";
                SqlCommand cmd = new SqlCommand(insertPolicySql, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@PolicyID", policyId);
                cmd.Parameters.AddWithValue("@Type", insuranceType);
                cmd.Parameters.AddWithValue("@Start", startDate);
                cmd.Parameters.AddWithValue("@End", endDate);
                cmd.Parameters.AddWithValue("@Nominee", nomineeName ?? "Not Provided");
                cmd.ExecuteNonQuery();
            }

            ViewBag.PolicyName = GetPolicyName(policyId);
            ViewBag.EMIMonths = emiMonths;
            ViewBag.TotalPremium = totalPremium;
            return View("PurchaseSuccess");
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
            PolicyViewModel policy = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT p.PolicyID, p.PolicyName, p.PlanKind, p.Description, p.DurationMonths, p.BasePremium,
                   pt.TypeName, c.CompanyName
            FROM Policies p
            INNER JOIN PolicyTypes pt ON p.PolicyTypeID = pt.PolicyTypeID
            INNER JOIN Clients c ON p.ClientID = c.ClientID
            WHERE p.PolicyID = @PolicyID";

                SqlCommand cmd = new SqlCommand(query, conn);
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

            return View(policy);
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
