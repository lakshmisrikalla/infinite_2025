using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using LTI.Models;
using System.Net;
using System.Net.Mail;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace LTI.Controllers
{
    public class AdminController : Controller
    {

        // Dashboard view
        public ActionResult Dashboard()
        {
            if (Session["AdminLoggedIn"] == null)
            {
                TempData["Error"] = "⚠️ Please log in to access the dashboard.";
                return RedirectToAction("Login", "Account");
            }

            var model = new AdminDashboardViewModel
            {
                Name = Session["AdminName"]?.ToString(),
                LoginID = Session["LoginID"]?.ToString(),
                LoginTime = Session["LoginTime"] != null
                            ? Convert.ToDateTime(Session["LoginTime"])
                            : DateTime.Now
            };

            return View(model);
        }

        //Manage users

        private readonly string _cs = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        [HttpGet]
        public ActionResult ManageUsers(string search, string status = "All", string blacklist = "All", int page = 1, int pageSize = 10)
        {
            var vm = new ManageUsersFilterVm
            {
                Search = search,
                Status = status,
                Blacklist = blacklist,
                Page = page,
                PageSize = pageSize
            };

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(@"
        SELECT u.LoginID, u.FullName, l.Email, u.PhoneNumber, u.Status,
               CASE WHEN b.LoginID IS NULL THEN 0 ELSE 1 END AS Blacklisted
        FROM Users u
        JOIN LoginCredentials l ON u.LoginID = l.LoginID AND l.Role = 'User'
        LEFT JOIN BlacklistedLogins b ON u.LoginID = b.LoginID
        WHERE (@search IS NULL OR u.FullName LIKE '%'+@search+'%' OR l.Email LIKE '%'+@search+'%' OR u.PhoneNumber LIKE '%'+@search+'%')
          AND (@status = 'All' OR u.Status = @status)
          AND (@blacklist = 'All' OR (@blacklist = 'Yes' AND b.LoginID IS NOT NULL) OR (@blacklist = 'No' AND b.LoginID IS NULL))
        ORDER BY u.FullName
        OFFSET (@page-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY;
 
        SELECT COUNT(*)
        FROM Users u
        JOIN LoginCredentials l ON u.LoginID = l.LoginID AND l.Role = 'User'
        LEFT JOIN BlacklistedLogins b ON u.LoginID = b.LoginID
        WHERE (@search IS NULL OR u.FullName LIKE '%'+@search+'%' OR l.Email LIKE '%'+@search+'%' OR u.PhoneNumber LIKE '%'+@search+'%')
          AND (@status = 'All' OR u.Status = @status)
          AND (@blacklist = 'All' OR (@blacklist = 'Yes' AND b.LoginID IS NOT NULL) OR (@blacklist = 'No' AND b.LoginID IS NULL));
    ", con))
            {
                cmd.Parameters.AddWithValue("@search", (object)search ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@status", status ?? "All");
                cmd.Parameters.AddWithValue("@blacklist", blacklist ?? "All");
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        vm.Items.Add(new UserListItemViewModel
                        {
                            LoginID = r.GetInt32(0),
                            Name = r.GetString(1),
                            Email = r.IsDBNull(2) ? null : r.GetString(2),
                            Phone = r.IsDBNull(3) ? null : r.GetString(3),
                            Status = r.GetString(4),
                            Blacklisted = r.GetInt32(5) == 1
                        });
                    }
                    if (r.NextResult() && r.Read())
                        vm.Total = r.GetInt32(0);
                }
            }
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BlockUser(int id)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                "UPDATE Users SET Status='Blocked' WHERE LoginID=@id; UPDATE LoginCredentials SET IsActive=0 WHERE LoginID=@id;", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            LogAdminAction("BlockUser", "User", id, "User account blocked");
            return RedirectToAction("ManageUsers");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnblockUser(int id)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                "UPDATE Users SET Status='Active' WHERE LoginID=@id; UPDATE LoginCredentials SET IsActive=1 WHERE LoginID=@id;", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            LogAdminAction("UnblockUser", "User", id, "User account unblocked");
            return RedirectToAction("ManageUsers");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BlacklistUser(int id, string reason)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(@"
IF NOT EXISTS(SELECT 1 FROM BlacklistedLogins WHERE LoginID=@id)
    INSERT INTO BlacklistedLogins(LoginID, Reason) VALUES(@id, @reason);", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@reason", (object)reason ?? DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            LogAdminAction("BlacklistUser", "User", id, "Reason: " + (reason ?? "-"));
            return RedirectToAction("ManageUsers");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WhitelistUser(int id)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("DELETE FROM BlacklistedLogins WHERE LoginID=@id;", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            LogAdminAction("WhitelistUser", "User", id, "Removed from blacklist");
            return RedirectToAction("ManageUsers");
        }

        [HttpGet]
        public ActionResult UserDetails(int id)
        {
            var vm = new UserDetailsViewModel();
            using (var con = new SqlConnection(_cs))
            {
                con.Open();

                using (var cmd = new SqlCommand(@"
SELECT u.LoginID, u.FullName, u.PhoneNumber, u.Address, u.DateOfBirth, u.Gender, u.Status, u.CreatedAt,
       l.Email, l.Username, l.IsActive, l.CreatedAt
FROM Users u
JOIN LoginCredentials l ON u.LoginID = l.LoginID
WHERE u.LoginID = @id;", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            vm.LoginID = r.GetInt32(0);
                            vm.FullName = r.IsDBNull(1) ? null : r.GetString(1);
                            vm.Phone = r.IsDBNull(2) ? null : r.GetString(2);
                            vm.Address = r.IsDBNull(3) ? null : r.GetString(3);
                            vm.DateOfBirth = r.IsDBNull(4) ? (DateTime?)null : r.GetDateTime(4);
                            vm.Gender = r.IsDBNull(5) ? null : r.GetString(5);
                            vm.UserStatus = r.IsDBNull(6) ? null : r.GetString(6);
                            vm.UserCreatedAt = r.IsDBNull(7) ? (DateTime?)null : r.GetDateTime(7);
                            vm.Email = r.IsDBNull(8) ? null : r.GetString(8);
                            vm.Username = r.IsDBNull(9) ? null : r.GetString(9);
                            vm.IsActive = !r.IsDBNull(10) && r.GetBoolean(10);
                            vm.LoginCreatedAt = r.IsDBNull(11) ? (DateTime?)null : r.GetDateTime(11);
                        }
                    }
                }

                using (var cmd = new SqlCommand(@"
SELECT up.UserPolicyID, up.PolicyID, p.PolicyName, pt.TypeName, c.CompanyName,
       up.StartDate, up.EndDate, up.Status, up.PaymentStatus
FROM UserPolicies up
JOIN Policies p ON up.PolicyID = p.PolicyID
JOIN PolicyTypes pt ON p.PolicyTypeID = pt.PolicyTypeID
JOIN Clients c ON p.ClientID = c.ClientID
WHERE up.UserID = (SELECT UserID FROM Users WHERE LoginID = @id)
ORDER BY up.CreatedAt DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            vm.Policies.Add(new UserPolicyRow
                            {
                                UserPolicyID = r.GetInt32(0),
                                PolicyID = r.GetInt32(1),
                                PolicyName = r.GetString(2),
                                PolicyType = r.GetString(3),
                                ClientName = r.GetString(4),
                                StartDate = r.GetDateTime(5),
                                EndDate = r.GetDateTime(6),
                                Status = r.GetString(7),
                                PaymentStatus = r.GetString(8)
                            });
                }

                using (var cmd = new SqlCommand(@"
SELECT pay.PaymentID, pay.UserPolicyID, pay.Amount, pay.Mode, pay.Status, pay.PaidAt, pay.GatewayRef
FROM Payments pay
JOIN UserPolicies up ON pay.UserPolicyID = up.UserPolicyID
WHERE up.UserID = (SELECT UserID FROM Users WHERE LoginID = @id)
ORDER BY pay.PaidAt DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            vm.Payments.Add(new PaymentRow
                            {
                                PaymentID = r.GetInt32(0),
                                UserPolicyID = r.GetInt32(1),
                                Amount = r.GetDecimal(2),
                                Mode = r.GetString(3),
                                Status = r.GetString(4),
                                PaidAt = r.GetDateTime(5),
                                GatewayRef = r.IsDBNull(6) ? null : r.GetString(6)
                            });
                }

                using (var cmd = new SqlCommand(@"
SELECT r.RenewalID, r.UserPolicyID, r.RequestedAt, r.ApprovedAt, r.Amount, r.Status
FROM Renewals r
JOIN UserPolicies up ON r.UserPolicyID = up.UserPolicyID
WHERE up.UserID = (SELECT UserID FROM Users WHERE LoginID = @id)
ORDER BY r.RequestedAt DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            vm.Renewals.Add(new RenewalRow
                            {
                                RenewalID = r.GetInt32(0),
                                UserPolicyID = r.GetInt32(1),
                                RequestedAt = r.GetDateTime(2),
                                ApprovedAt = r.IsDBNull(3) ? (DateTime?)null : r.GetDateTime(3),
                                Amount = r.GetDecimal(4),
                                Status = r.GetString(5)
                            });
                }

                using (var cmd = new SqlCommand(@"
SELECT c.ClaimID, c.UserPolicyID, c.ClaimDate, c.ClaimType, c.Reason, c.ClaimedAmount, c.Status, c.ApprovedAmount, c.DecisionAt
FROM Claims c
JOIN UserPolicies up ON c.UserPolicyID = up.UserPolicyID
WHERE up.UserID = (SELECT UserID FROM Users WHERE LoginID = @id)
ORDER BY c.ClaimDate DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            vm.Claims.Add(new ClaimRow
                            {
                                ClaimID = r.GetInt32(0),
                                UserPolicyID = r.GetInt32(1),
                                ClaimDate = r.GetDateTime(2),
                                ClaimType = r.GetString(3),
                                Reason = r.IsDBNull(4) ? null : r.GetString(4),
                                ClaimedAmount = r.GetDecimal(5),
                                Status = r.GetString(6),
                                ApprovedAmount = r.IsDBNull(7) ? (decimal?)null : r.GetDecimal(7),
                                DecisionAt = r.IsDBNull(8) ? (DateTime?)null : r.GetDateTime(8)
                            });
                }

                using (var cmd = new SqlCommand(@"
SELECT DocumentID, OwnerType, OwnerID, DocumentType, FilePath, IsVerified, VerifiedByRole, VerifiedByID, UploadedAt, VerifiedAt, Visibility
FROM Documents
WHERE (OwnerType='User' AND OwnerID = (SELECT UserID FROM Users WHERE LoginID=@id))
   OR (OwnerType='UserPolicy' AND OwnerID IN (
           SELECT up.UserPolicyID FROM UserPolicies up WHERE up.UserID = (SELECT UserID FROM Users WHERE LoginID=@id)
       ))
ORDER BY UploadedAt DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            vm.Documents.Add(new DocumentRow
                            {
                                DocumentID = r.GetInt32(0),
                                OwnerType = r.GetString(1),
                                OwnerID = r.GetInt32(2),
                                DocumentType = r.GetString(3),
                                FilePath = r.GetString(4),
                                IsVerified = r.GetBoolean(5),
                                VerifiedByRole = r.IsDBNull(6) ? null : r.GetString(6),
                                VerifiedByID = r.IsDBNull(7) ? (int?)null : r.GetInt32(7),
                                UploadedAt = r.GetDateTime(8),
                                VerifiedAt = r.IsDBNull(9) ? (DateTime?)null : r.GetDateTime(9),
                                Visibility = r.GetString(10)
                            });
                }
            }
            return View(vm);
        }

        //Reports and analytics-----------------------------------------------

        // ---------------- Reports & Analytics (GET) ----------------
        [HttpGet]
        public ActionResult ReportsAnalytics(string period = "Monthly", DateTime? startDate = null, DateTime? endDate = null)
        {
            // Default window: last 6 months ending today
            var utcToday = DateTime.UtcNow.Date;
            var start = startDate?.Date ?? new DateTime(utcToday.Year, utcToday.Month, 1).AddMonths(-5);
            var end = endDate?.Date ?? utcToday;
            if (end < start) end = start;

            var vm = new ReportsAnalyticsVm
            {
                SelectedPeriod = period,
                StartDate = start,
                EndDate = end
            };

            // Helpers for SQL grouping expressions -------------------
            string GroupExprFor(string alias, string dateCol)
            {
                var p = (period ?? "Monthly").ToLowerInvariant();
                switch (p)
                {
                    case "daily":
                        // e.g. CAST(CONVERT(date, pay.PaidAt) AS date)
                        return $"CAST(CONVERT(date, {alias}.{dateCol}) AS date)";
                    case "weekly":
                        // week start (Sunday): DATEADD(WEEK, DATEDIFF(WEEK,0,dt), 0)
                        return $"CAST(DATEADD(WEEK, DATEDIFF(WEEK, 0, {alias}.{dateCol}), 0) AS date)";
                    default: // monthly
                        return $"CAST(DATEFROMPARTS(YEAR({alias}.{dateCol}), MONTH({alias}.{dateCol}), 1) AS date)";
                }
            }
            string LabelExpr(string groupedDateExpr)
            {
                // yyyy-MM-dd
                return $"CONVERT(varchar(10), {groupedDateExpr}, 23)";
            }
            var endPlus = end.AddDays(1); // make end inclusive

            using (var con = new SqlConnection(_cs))
            {
                con.Open();

                // ---------- KPI: Users ----------
                using (var cmd = new SqlCommand(@"
            SELECT COUNT(*)                          -- total
                 , SUM(CASE WHEN Status='Active'  THEN 1 ELSE 0 END) as ActiveCnt
                 , SUM(CASE WHEN Status='Blocked' THEN 1 ELSE 0 END) as BlockedCnt
            FROM Users;", con))
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        vm.TotalUsers = r.IsDBNull(0) ? 0 : r.GetInt32(0);
                        vm.ActiveUsers = r.IsDBNull(1) ? 0 : Convert.ToInt32(r.GetValue(1));
                        vm.BlockedUsers = r.IsDBNull(2) ? 0 : Convert.ToInt32(r.GetValue(2));
                    }
                }

                // ---------- KPI: Clients ----------
                using (var cmd = new SqlCommand(@"
            SELECT COUNT(*),
                   SUM(CASE WHEN Status='Approved' THEN 1 ELSE 0 END),
                   SUM(CASE WHEN Status='Pending'  THEN 1 ELSE 0 END),
                   SUM(CASE WHEN Status='Blocked'  THEN 1 ELSE 0 END)
            FROM Clients;", con))
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        vm.TotalClients = r.IsDBNull(0) ? 0 : r.GetInt32(0);
                        vm.ApprovedClients = r.IsDBNull(1) ? 0 : Convert.ToInt32(r.GetValue(1));
                        vm.PendingClients = r.IsDBNull(2) ? 0 : Convert.ToInt32(r.GetValue(2));
                        vm.BlockedClients = r.IsDBNull(3) ? 0 : Convert.ToInt32(r.GetValue(3));
                    }
                }

                // ---------- KPI: Policies ----------
                using (var cmd = new SqlCommand(@"
            SELECT COUNT(*),
                   SUM(CASE WHEN Status='Approved' THEN 1 ELSE 0 END),
                   SUM(CASE WHEN Status='Pending'  THEN 1 ELSE 0 END),
                   SUM(CASE WHEN Status='Rejected' THEN 1 ELSE 0 END)
            FROM Policies;", con))
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        vm.TotalPolicies = r.IsDBNull(0) ? 0 : r.GetInt32(0);
                        vm.ApprovedPolicies = r.IsDBNull(1) ? 0 : Convert.ToInt32(r.GetValue(1));
                        vm.PendingPolicies = r.IsDBNull(2) ? 0 : Convert.ToInt32(r.GetValue(2));
                        vm.RejectedPolicies = r.IsDBNull(3) ? 0 : Convert.ToInt32(r.GetValue(3)); // reuse field
                    }
                }

                // ---------- KPI: Total Revenue ----------
                using (var cmd = new SqlCommand(@"
            SELECT ISNULL(SUM(Amount),0)
            FROM Payments
            WHERE Status='Success' AND PaidAt >= @s AND PaidAt < @e;", con))
                {
                    cmd.Parameters.AddWithValue("@s", start);
                    cmd.Parameters.AddWithValue("@e", endPlus);
                    vm.TotalRevenue = Convert.ToDecimal(cmd.ExecuteScalar() ?? 0m);
                }

                // ---------- KPI + Pie: Claims ----------
                using (var cmd = new SqlCommand(@"
            SELECT 
              COUNT(*) as Total,
              SUM(CASE WHEN Status='Approved'    THEN 1 ELSE 0 END),
              SUM(CASE WHEN Status='Rejected'    THEN 1 ELSE 0 END),
              SUM(CASE WHEN Status='UnderReview' THEN 1 ELSE 0 END),
              SUM(CASE WHEN Status='Settled'     THEN 1 ELSE 0 END),
              SUM(CASE WHEN Status='Submitted'   THEN 1 ELSE 0 END)
            FROM Claims
            WHERE ClaimDate >= @s AND ClaimDate < @e;", con))
                {
                    cmd.Parameters.AddWithValue("@s", start);
                    cmd.Parameters.AddWithValue("@e", endPlus);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            vm.TotalClaims = r.IsDBNull(0) ? 0 : r.GetInt32(0);
                            vm.ApprovedClaims = r.IsDBNull(1) ? 0 : Convert.ToInt32(r.GetValue(1));
                            vm.RejectedClaims = r.IsDBNull(2) ? 0 : Convert.ToInt32(r.GetValue(2));
                            vm.UnderReviewClaims = r.IsDBNull(3) ? 0 : Convert.ToInt32(r.GetValue(3));
                            vm.SettledClaims = r.IsDBNull(4) ? 0 : Convert.ToInt32(r.GetValue(4));
                            vm.SubmittedClaims = r.IsDBNull(5) ? 0 : Convert.ToInt32(r.GetValue(5));
                        }
                    }
                }

                // ---------- Revenue over time (bar/line) ----------
                var gPay = GroupExprFor("pay", "PaidAt");
                var lblPay = LabelExpr(gPay);
                var sqlRev = $@"
            SELECT {lblPay} AS Label, SUM(pay.Amount) AS Total
            FROM Payments pay
            WHERE pay.Status='Success' AND pay.PaidAt >= @s AND pay.PaidAt < @e
            GROUP BY {gPay}
            ORDER BY {gPay};";

                using (var cmd = new SqlCommand(sqlRev, con))
                {
                    cmd.Parameters.AddWithValue("@s", start);
                    cmd.Parameters.AddWithValue("@e", endPlus);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            vm.RevenueMonthLabels.Add(r.GetString(0));
                            vm.RevenueMonthValues.Add(r.IsDBNull(1) ? 0m : r.GetDecimal(1));
                        }
                    }
                }

                // ---------- New users over time ----------
                var gUsr = GroupExprFor("u", "CreatedAt");
                var lblUsr = LabelExpr(gUsr);
                var sqlUsers = $@"
            SELECT {lblUsr} AS Label, COUNT(*) AS Cnt
            FROM Users u
            WHERE u.CreatedAt >= @s AND u.CreatedAt < @e
            GROUP BY {gUsr}
            ORDER BY {gUsr};";

                using (var cmd = new SqlCommand(sqlUsers, con))
                {
                    cmd.Parameters.AddWithValue("@s", start);
                    cmd.Parameters.AddWithValue("@e", endPlus);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            vm.NewUsersMonthLabels.Add(r.GetString(0));
                            vm.NewUsersMonthValues.Add(r.GetInt32(1));
                        }
                    }
                }

                // ---------- Pie: Clients by status ----------
                vm.ClientStatusLabels.AddRange(new[] { "Approved", "Pending", "Blocked" });
                using (var cmd = new SqlCommand(@"
            SELECT 
              SUM(CASE WHEN Status='Approved' THEN 1 ELSE 0 END),
              SUM(CASE WHEN Status='Pending'  THEN 1 ELSE 0 END),
              SUM(CASE WHEN Status='Blocked'  THEN 1 ELSE 0 END)
            FROM Clients;", con))
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        vm.ClientStatusValues.Add(r.IsDBNull(0) ? 0 : Convert.ToInt32(r.GetValue(0)));
                        vm.ClientStatusValues.Add(r.IsDBNull(1) ? 0 : Convert.ToInt32(r.GetValue(1)));
                        vm.ClientStatusValues.Add(r.IsDBNull(2) ? 0 : Convert.ToInt32(r.GetValue(2)));
                    }
                }

                // ---------- Pie: Policies by status ----------
                vm.PolicyStatusLabels.AddRange(new[] { "Approved", "Pending", "Rejected" });
                using (var cmd = new SqlCommand(@"
            SELECT 
              SUM(CASE WHEN Status='Approved' THEN 1 ELSE 0 END),
              SUM(CASE WHEN Status='Pending'  THEN 1 ELSE 0 END),
              SUM(CASE WHEN Status='Rejected' THEN 1 ELSE 0 END)
            FROM Policies;", con))
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        vm.PolicyStatusValues.Add(r.IsDBNull(0) ? 0 : Convert.ToInt32(r.GetValue(0)));
                        vm.PolicyStatusValues.Add(r.IsDBNull(1) ? 0 : Convert.ToInt32(r.GetValue(1)));
                        vm.PolicyStatusValues.Add(r.IsDBNull(2) ? 0 : Convert.ToInt32(r.GetValue(2)));
                    }
                }

                // ---------- Pie: Claims by status ----------
                vm.ClaimStatusLabels.AddRange(new[] { "Approved", "Rejected", "UnderReview", "Settled", "Submitted" });
                vm.ClaimStatusValues.AddRange(new[] { vm.ApprovedClaims, vm.RejectedClaims, vm.UnderReviewClaims, vm.SettledClaims, vm.SubmittedClaims });
            }

            return View(vm);
        }

        //------------------------------------------------------------------------------------

        //Admin history---------------------------------
        [HttpGet]
        public ActionResult AdminHistory()
        {
            var list = new List<AuditLogVm>();
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
        SELECT TOP 200 a.AuditID, ad.FullName, a.ActionType, a.TargetType, a.TargetID, a.Details, a.CreatedAt
        FROM AdminAuditLogs a
        JOIN Admins ad ON a.AdminID = ad.AdminID
        ORDER BY a.CreatedAt DESC;", con))
            {
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new AuditLogVm
                        {
                            AuditID = r.GetInt32(0),
                            AdminName = r.GetString(1),
                            ActionType = r.GetString(2),
                            TargetType = r.IsDBNull(3) ? null : r.GetString(3),
                            TargetID = r.IsDBNull(4) ? (int?)null : r.GetInt32(4),
                            Details = r.IsDBNull(5) ? null : r.GetString(5),
                            CreatedAt = r.GetDateTime(6)
                        });
                    }
                }
            }

            return View(list);
        }






        private int GetCurrentAdminId()
        {
            var loginIdObj = Session["LoginID"];
            if (loginIdObj == null) return 0;
            var loginId = Convert.ToInt32(loginIdObj);

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("SELECT AdminID FROM Admins WHERE LoginID=@lid;", con))
            {
                cmd.Parameters.AddWithValue("@lid", loginId);
                con.Open();
                var res = cmd.ExecuteScalar();
                return (res == null || res == DBNull.Value) ? 0 : Convert.ToInt32(res);
            }
        }

        private void LogAdminAction(string actionType, string targetType, int? targetId, string details = null)
        {
            var adminId = GetCurrentAdminId();
            if (adminId <= 0) return;

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(@"
INSERT INTO AdminAuditLogs(AdminID, ActionType, TargetType, TargetID, Details)
VALUES(@a, @act, @tt, @tid, @d);", con))
            {
                cmd.Parameters.AddWithValue("@a", adminId);
                cmd.Parameters.AddWithValue("@act", actionType);
                cmd.Parameters.AddWithValue("@tt", (object)targetType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@tid", (object)targetId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@d", (object)details ?? DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //--------------------------------------------------------------------------------------------------------------
        public ActionResult ClientDocumentManagement()
        {
            return View(); // Create ClientDocumentManagement.cshtml
        }

        public ActionResult CommunicationNotifications()
        {
            return View(); // Create CommunicationNotifications.cshtml
        }
    

        // ===========================
        // CLIENT & DOCUMENT MANAGEMENT
        // ===========================

        // 1. Manage Clients
        public ActionResult ManageClients(string status = null)
        {
            List<ClientViewModel> clients = new List<ClientViewModel>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string query = @"
            SELECT c.ClientID, c.CompanyName, c.CompanyCode, c.ContactEmail, c.ContactPhone, c.Status,
                   d.DocumentType, d.FilePath, d.DocumentID
            FROM Clients c
            LEFT JOIN Documents d ON d.OwnerID = c.ClientID AND d.OwnerType = 'Client'";

                if (!string.IsNullOrEmpty(status))
                {
                    query += " WHERE c.Status = @Status";
                }

                SqlCommand cmd = new SqlCommand(query, con);
                if (!string.IsNullOrEmpty(status))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                }

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    clients.Add(new ClientViewModel
                    {
                        ClientID = (int)reader["ClientID"],
                        CompanyName = reader["CompanyName"].ToString(),
                        CompanyCode = reader["CompanyCode"].ToString(),
                        ContactEmail = reader["ContactEmail"].ToString(),
                        ContactPhone = reader["ContactPhone"].ToString(),
                        Status = reader["Status"].ToString(),
                        DocumentType = reader["DocumentType"] != DBNull.Value ? reader["DocumentType"].ToString() : null,
                        FilePath = reader["FilePath"] != DBNull.Value ? reader["FilePath"].ToString() : null,
                        DocumentID = reader["DocumentID"] != DBNull.Value ? (int)reader["DocumentID"] : 0
                    });
                }
                con.Close();
            }
            return View(clients);
        }

        [HttpPost]
        public ActionResult UpdateVerificationStatus(int clientId, bool approve)
        {
            // Determine the new status based on the button clicked
            string newStatus = approve ? "Approved" : "Blocked";


            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                // Update the client's status in the database
                string query = "UPDATE Clients SET Status = @Status WHERE ClientID = @ClientID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Status", newStatus);
                cmd.Parameters.AddWithValue("@ClientID", clientId);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            // Redirect back to the ManageClients view to reflect changes
            return RedirectToAction("ManageClients");
        }


        [HttpGet]
        public ActionResult ApplyDynamicFilters()
        {
            return View(new ClientFilterViewModel { FilteredClients = new List<ClientViewModel>() });
        }

        [HttpPost]
        public ActionResult ApplyDynamicFilters(ClientFilterViewModel filters)
        {
            List<ClientViewModel> clients = new List<ClientViewModel>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<string> conditions = new List<string>();
                SqlCommand cmd = new SqlCommand();
                string query = "SELECT ClientID, CompanyName, CompanyCode, ContactEmail, ContactPhone, Status, RegisteredAt FROM Clients";

                if (!string.IsNullOrEmpty(filters.Status) && filters.Status != "All")
                {
                    conditions.Add("Status = @Status");
                    cmd.Parameters.AddWithValue("@Status", filters.Status);
                }


                if (filters.RegisteredAfter.HasValue)
                {
                    conditions.Add("RegisteredAt >= @RegisteredAfter");
                    cmd.Parameters.AddWithValue("@RegisteredAfter", filters.RegisteredAfter.Value);
                }

                // If you want to filter by verification, you can join with Documents table or add IsVerified column to Clients

                if (conditions.Count > 0)
                {
                    query += " WHERE " + string.Join(" AND ", conditions);
                }

                cmd.CommandText = query;
                cmd.Connection = con;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    clients.Add(new ClientViewModel
                    {
                        ClientID = (int)reader["ClientID"],
                        CompanyName = reader["CompanyName"].ToString(),
                        CompanyCode = reader["CompanyCode"].ToString(),
                        ContactEmail = reader["ContactEmail"].ToString(),
                        ContactPhone = reader["ContactPhone"].ToString(),
                        Status = reader["Status"].ToString(),
                        RegisteredAt = (DateTime)reader["RegisteredAt"]
                    });
                }
                con.Close();
            }

            filters.FilteredClients = clients;
            return View(filters);
        }
        [HttpGet]
        public ActionResult ManageEmails()
        {
            var model = new AdminEmailModel();
            ViewBag.EmailTypes = new List<string>();
            ViewBag.RecipientEmails = new List<string>();
            return View(model);
        }
        [HttpPost]
        public ActionResult ManageEmails(AdminEmailModel model)
        {
            string body = model.EmailType == "Custom" ? model.CustomBody : GetEmailBody(model.RecipientType, model.EmailType);
            string subject = model.EmailType;
            bool success = SendEmail(model.ToEmail, subject, body, out string error);
            if (success)
            {
                LogSentEmail(model.RecipientType, model.RecipientID, subject, body, model.EmailType);
            }
            ViewBag.Status = success ? "✅ Email sent successfully!" : $"Fetching Details... ";
            ViewBag.EmailTypes = GetEmailTypes(model.RecipientType);
            ViewBag.RecipientEmails = GetRecipientEmails(model.RecipientType);
            return View(model);
        }

        private List<string> GetRecipientEmails(string recipientType)

        {

            var emails = new List<string>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))

            {

                string query = "SELECT Email FROM LoginCredentials WHERE Role = @Role";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Role", recipientType);

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())

                {

                    emails.Add(reader["Email"].ToString());

                }

            }

            return emails;

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

        private string GetEmailBody(string recipientType, string emailType)
        {
            if (recipientType == "User" && emailType == "Welcome Mail")
                return "Hi User,We're thrilled to have you on board! Explore your dashboard and get started today.Cheers,LTI Team";
            if (recipientType == "User" && emailType == "Verification Mail")
                return "Hi User, welcome to our platform!Please co-operate to our verification process";
            if (recipientType == "Client" && emailType == "Onboarding Mail")
                return "Dear Client, your onboarding process has started.";
            if (recipientType == "Client" && emailType == "Document Reminder")
                return "Dear Client, This is a remainder to submit your documents.";
            if (recipientType == "Client" && emailType == "Policy Update")
                return "Dear Client, your policy updated successfully.";
            if (recipientType == "Client" && emailType == "Promotional Offer")
                return "Hi Client, " +
                    "Unlock 20% off your next upgrade! Use code WELCOME20 at checkout.Offer valid till tomorrow.";

            return "Default email body.";
        }

        private List<string> GetEmailTypes(string recipientType)
        {
            var templates = new List<string>();
            if (recipientType == "User")
                templates.AddRange(new[] { "Welcome Mail", "Verification Mail" });
            if (recipientType == "Client")
                templates.AddRange(new[] { "Onboarding Mail", "Document Reminder", "Policy Update", "Promotional Offer" });

            templates.Add("Custom"); // Add Custom option for both
            return templates;
        }
        private void LogSentEmail(string recipientType, int recipientID, string subject, string body, string emailType)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string query = @"INSERT INTO EmailLogs (RecipientType, RecipientID, Subject, Body, EmailType, SentAt)
                         VALUES (@RecipientType, @RecipientID, @Subject, @Body, @EmailType, SYSUTCDATETIME())";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RecipientType", recipientType);
                cmd.Parameters.AddWithValue("@RecipientID", recipientID);
                cmd.Parameters.AddWithValue("@Subject", subject);
                cmd.Parameters.AddWithValue("@Body", body);
                cmd.Parameters.AddWithValue("@EmailType", emailType);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<EmailLog> GetMinimalEmailLogs()
        {
            var emails = new List<EmailLog>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string query = "SELECT RecipientType, EmailType, SentAt FROM EmailLogs ORDER BY SentAt DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    emails.Add(new EmailLog
                    {
                        RecipientType = reader["RecipientType"].ToString(),
                        EmailType = reader["EmailType"].ToString(),
                        SentAt = (DateTime)reader["SentAt"]
                    });
                }
            }
            return emails;
        }

        public ActionResult ViewAllEmails()
        {
            var emails = GetMinimalEmailLogs();
            return View(emails);
        }

        
    }
}
