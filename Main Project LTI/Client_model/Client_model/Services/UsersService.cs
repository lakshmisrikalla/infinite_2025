using System.Collections.Generic;
using System.Linq;
using System.Data.Entity; // for Include
using Client_model.Models;

namespace Client_model.Services
{
    public class UsersService
    {
        private readonly InsuranceDB1Entities _db;
        public UsersService(InsuranceDB1Entities db) { _db = db; }

        public IEnumerable<PolicyHolderVm> GetPolicyHolders(int clientId)
        {
            return _db.UserPolicies
                      .Include(up => up.User)
                      .Include(up => up.Policy)
                      .Where(up => up.Policy.ClientID == clientId)
                      .Select(up => new PolicyHolderVm
                      {
                          UserPolicyID = up.UserPolicyID,
                          FullName = up.User.FullName,
                          Policy = up.Policy.PolicyName,
                          Status = up.Status,
                          PaymentStatus = up.PaymentStatus,
                          StartDate = up.StartDate,
                          EndDate = up.EndDate,

                          VehicleDetails = up.VehicleDetails.Select(v => new VehicleDetailVm
                          {
                              VehicleType = v.VehicleType,
                              VehicleName = v.VehicleName,
                              Model = v.Model,
                              RegistrationNumber = v.RegistrationNumber,
                              DrivingLicense = v.DrivingLicense,
                              ExpiryDate = v.ExpiryDate,
                              HolderName = v.HolderName
                          }).ToList(),

                          TravelDetails = up.TravelDetails.Select(t => new TravelDetailVm
                          {
                              PersonName = t.PersonName,
                              Age = t.Age,
                              Gender = t.Gender,
                              DOB = t.DOB,
                              HealthIssues = t.HealthIssues
                          }).ToList()
                      })
                      .ToList();
        }
    }
}
