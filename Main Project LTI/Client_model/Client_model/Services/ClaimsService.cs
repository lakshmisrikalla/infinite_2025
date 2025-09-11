using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Client_model.Models;

namespace Client_model.Services
{
    public class ClaimsService
    {
        private readonly InsuranceDB1Entities _db;
        public ClaimsService(InsuranceDB1Entities db) { _db = db; }

        public IEnumerable<Claim> GetClaims(int clientId)
        {
            return _db.Claims
                .Include("UserPolicy.Policy")
                .Include("UserPolicy.User")
                .Where(c => c.UserPolicy.Policy.ClientID == clientId)
                .OrderByDescending(c => c.ClaimDate)
                .ToList();
        }

        public Claim GetClaimById(int claimId)
        {
            return _db.Claims
                .Include("UserPolicy.Policy")
                .Include("UserPolicy.User")
                .FirstOrDefault(c => c.ClaimID == claimId);
        }

        public void UpdateClaim(Claim claim)
        {
            _db.Entry(claim).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }
    }
}
