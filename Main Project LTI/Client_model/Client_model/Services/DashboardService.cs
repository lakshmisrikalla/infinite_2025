using System;
using System.Linq;
using System.Collections.Generic;
using Client_model.Models;

namespace Client_model.Services
{
    public class DashboardService
    {
        private readonly InsuranceDB1Entities _db;
        public DashboardService(InsuranceDB1Entities db) { _db = db; }

        public DashboardSummaryVm GetSummary(int clientId)
        {
            var today = DateTime.UtcNow.Date;
            var next30 = today.AddDays(30);

            // Policies owned by this client
            var clientPolicies = _db.Policies
                .Where(p => p.ClientID == clientId && p.IsActive);

            // All user policies under those policies
            var userPolicies = _db.UserPolicies
                .Where(up => clientPolicies.Select(p => p.PolicyID).Contains(up.PolicyID));

            // Successful payments for those user policies
            var paidPayments = _db.Payments
                .Where(p => userPolicies.Select(up => up.UserPolicyID).Contains(p.UserPolicyID)
                         && p.Status == "Success");

            // Summary cards
            var summary = new DashboardSummaryVm
            {
                TotalPolicies = clientPolicies.Count(),
                ActivePolicies = clientPolicies.Count(p => p.ClientID == clientId && p.IsActive),
                PolicyHolders = userPolicies.Select(up => up.UserID).Distinct().Count(),
                ClaimsCount = _db.Claims.Count(c => userPolicies.Select(up => up.UserPolicyID).Contains(c.UserPolicyID)),
                PendingApprovals = userPolicies.Count(up => up.Status == "Pending"),
                PendingRenewals = _db.Renewals.Count(r => userPolicies.Select(up => up.UserPolicyID).Contains(r.UserPolicyID) && r.Status == "Pending"),
                DueSoon = _db.PolicyDueDates.Count(d =>
                              userPolicies.Select(up => up.UserPolicyID).Contains(d.UserPolicyID) &&
                              d.DueDate >= today && d.DueDate <= next30 && d.IsPaid == false),
                CollectedPayments = paidPayments.Any() ? paidPayments.Sum(p => p.Amount) : 0m
            };

            // ----- Revenue (last 6 months) -----
            var startMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-5);

            // Group in SQL by Year/Month (EF-safe), then format in memory
            var revenueRaw = paidPayments
                .Where(p => p.PaidAt >= startMonth)
                .GroupBy(p => new { p.PaidAt.Year, p.PaidAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(x => x.Amount) })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList(); // materialize before string formatting

            // Ensure all 6 months are present (fill missing with 0)
            var months = Enumerable.Range(0, 6).Select(i => startMonth.AddMonths(i)).ToList();
            var revenueSeries = new List<SeriesPoint>();
            foreach (var m in months)
            {
                var match = revenueRaw.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);
                var total = match != null ? match.Total : 0m;
                revenueSeries.Add(new SeriesPoint { Label = m.ToString("yyyy-MM"), Value = total });
            }
            summary.RevenueByMonth = revenueSeries;

            // ----- Top policies by holder count (Top 5) -----
            summary.TopPolicies = userPolicies
                .GroupBy(up => up.Policy.PolicyName)
                .Select(g => new SeriesPoint { Label = g.Key, Value = g.Count() })
                .OrderByDescending(x => x.Value)
                .Take(5)
                .ToList();

            return summary;
        }
    }
}
