using System;
using System.Collections.Generic;

namespace Client_model.Models
{
    public class PolicyHolderVm
    {
      // Basic user/policy info
        public int UserPolicyID { get; set; }
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Policy { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Details collections
        public List<VehicleDetailVm> VehicleDetails { get; set; }
        public List<TravelDetailVm> TravelDetails { get; set; }
    }

    public class VehicleDetailVm
    {
        public string VehicleType { get; set; }
        public string VehicleName { get; set; }
        public string Model { get; set; }
        public string RegistrationNumber { get; set; }
        public string DrivingLicense { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string HolderName { get; set; }
    }

    public class TravelDetailVm
    {
        public string PersonName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string HealthIssues { get; set; }
    }
}