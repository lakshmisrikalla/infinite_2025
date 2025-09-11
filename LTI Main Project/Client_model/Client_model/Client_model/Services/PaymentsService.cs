using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Client_model.Models;

namespace Client_model.Services
{
    public class PaymentsService
    {
        private readonly InsuranceDB1Entities _db;
        public PaymentsService(InsuranceDB1Entities db) { _db = db; }

        /// <summary>
        /// Returns payments for policies that belong to clientId.
        /// Projects into PaymentVm to avoid dynamic proxies.
        /// </summary>
        public IEnumerable<PaymentVm> GetPaymentsForClient(int clientId)
        {
            var q = _db.Payments
                       .Include(p => p.UserPolicy)
                       .Include(p => p.UserPolicy.User)
                       .Include(p => p.UserPolicy.Policy)
                       .Where(p => p.UserPolicy.Policy.ClientID == clientId)
                       .OrderByDescending(p => p.PaidAt)
                       .Select(p => new PaymentVm
                       {
                           PaymentID = p.PaymentID,
                           UserPolicyID = p.UserPolicyID,
                           Holder = p.UserPolicy.User.FullName,
                           Policy = p.UserPolicy.Policy.PolicyName,
                           Amount = p.Amount,
                           Mode = p.Mode,
                           Status = p.Status,
                           PaidAt = p.PaidAt,
                           GatewayRef = p.GatewayRef
                       });

            return q.ToList();
        }
    }
}
