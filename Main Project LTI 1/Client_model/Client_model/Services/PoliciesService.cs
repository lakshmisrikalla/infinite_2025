using System.Linq;
using Client_model.Models;

namespace Client_model.Services
{
    public class PoliciesService
    {
        private readonly InsuranceDB1Entities _db;
        public PoliciesService(InsuranceDB1Entities db) { _db = db; }

        public IQueryable<Policy> GetClientPolicies(int clientId)
        {
            return _db.Policies.Where(p => p.ClientID == clientId).OrderByDescending(p => p.CreatedAt);
        }
    }
}
