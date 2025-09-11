using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Client_model.Models;

namespace Client_model.Services
{
    public class ReportsService
    {
        private readonly InsuranceDB1Entities _db;

        public ReportsService(InsuranceDB1Entities db)
        {
            _db = db;
        }

        public ReportsPageVm GetReportsForClient(int clientId, DateTime? from = null, DateTime? to = null, int? year = null, int? quarter = null)
        {
            if (year.HasValue && quarter.HasValue)
            {
                var start = new DateTime(year.Value, (quarter.Value - 1) * 3 + 1, 1);
                from = start;
                to = start.AddMonths(3).AddDays(-1);
            }
            else if (year.HasValue && !from.HasValue && !to.HasValue)
            {
                from = new DateTime(year.Value, 1, 1);
                to = new DateTime(year.Value, 12, 31);
            }

            if (!from.HasValue && !to.HasValue)
            {
                to = DateTime.UtcNow.Date;
                from = to.Value.AddMonths(-12).AddDays(1);
            }

            var userPoliciesQ = _db.UserPolicies
                .Include(up => up.Policy)
                .Where(up => up.Policy.ClientID == clientId);

            var paymentsQ = _db.Payments
                .Include(p => p.UserPolicy)
                .Include(p => p.UserPolicy.Policy)
                .Where(p => p.UserPolicy.Policy.ClientID == clientId);

            var claimsQ = _db.Claims
                .Include(c => c.UserPolicy)
                .Where(c => c.UserPolicy.Policy.ClientID == clientId);

            var renewalsQ = _db.Renewals
                .Include(r => r.UserPolicy)
                .Where(r => r.UserPolicy.Policy.ClientID == clientId);

            var duesQ = _db.PolicyDueDates
                .Include(d => d.UserPolicy)
                .Where(d => d.UserPolicy.Policy.ClientID == clientId);

            if (from.HasValue)
            {
                paymentsQ = paymentsQ.Where(p => p.PaidAt >= from.Value);
                userPoliciesQ = userPoliciesQ.Where(up => up.StartDate >= from.Value || up.CreatedAt >= from.Value);
                claimsQ = claimsQ.Where(c => c.ClaimDate >= from.Value);
                renewalsQ = renewalsQ.Where(r => r.RequestedAt >= from.Value);
                duesQ = duesQ.Where(d => d.DueDate >= from.Value);
            }
            if (to.HasValue)
            {
                paymentsQ = paymentsQ.Where(p => p.PaidAt <= to.Value);
                userPoliciesQ = userPoliciesQ.Where(up => up.EndDate <= to.Value || up.CreatedAt <= to.Value);
                claimsQ = claimsQ.Where(c => c.ClaimDate <= to.Value);
                renewalsQ = renewalsQ.Where(r => r.RequestedAt <= to.Value);
                duesQ = duesQ.Where(d => d.DueDate <= to.Value);
            }

            var totalPayments = paymentsQ.Where(p => p.Status == "Success").Sum(p => (decimal?)p.Amount) ?? 0m;
            var totalPolicyCount = userPoliciesQ.Count();
            var totalClaims = claimsQ.Count();
            var totalRenewals = renewalsQ.Count();

            var topExpensivePolicies = _db.Policies
                .Where(p => p.ClientID == clientId && p.IsActive)  // Filter by IsActive boolean
                .OrderByDescending(p => p.BasePremium)
                .Take(5)
                .Select(p => new TopPolicyVm
                {
                    PolicyID = p.PolicyID,
                    PolicyName = p.PolicyName,
                    TotalCollected = paymentsQ.Where(pay => pay.UserPolicy.PolicyID == p.PolicyID && pay.Status == "Success")
                                             .Sum(pay => (decimal?)pay.Amount) ?? 0m,
                    BuyersCount = userPoliciesQ.Count(up => up.PolicyID == p.PolicyID)
                })
                .ToList();

            var lowPolicies = _db.Policies
                .Where(p => p.ClientID == clientId && p.IsActive)  // Filter by IsActive boolean
                .OrderBy(p => p.BasePremium)
                .Take(5)
                .Select(p => new TopPolicyVm
                {
                    PolicyID = p.PolicyID,
                    PolicyName = p.PolicyName,
                    TotalCollected = paymentsQ.Where(pay => pay.UserPolicy.PolicyID == p.PolicyID && pay.Status == "Success")
                                             .Sum(pay => (decimal?)pay.Amount) ?? 0m,
                    BuyersCount = userPoliciesQ.Count(up => up.PolicyID == p.PolicyID)
                })
                .ToList();

            var vm = new ReportsPageVm
            {
                From = from,
                To = to,
                Year = year,
                Quarter = quarter,
                TotalAmount = totalPayments,
                TotalBuyers = totalPolicyCount,
                PoliciesSold = totalPolicyCount,
                ClaimsCount = totalClaims,
                RenewalsCount = totalRenewals,
                DueCount = duesQ.Count(d => !d.IsPaid),
                TopPoliciesByAmount = topExpensivePolicies,
                LowPoliciesByAmount = lowPolicies
            };

            return vm;
        }
    }
}
