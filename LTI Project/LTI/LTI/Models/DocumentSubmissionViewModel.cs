using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class DocumentSubmissionViewModel
    {
        public int PolicyID { get; set; }
        public int UserPolicyID { get; set; }
        public string PolicyName { get; set; }
        public string PolicyType { get; set; }

        // Travel
        public string PersonName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string HealthIssues { get; set; }

        // Motor
        public string VehicleType { get; set; }
        public string VehicleName { get; set; }
        public string Model { get; set; }
        public string DrivingLicense { get; set; }
        public string RegistrationNumber { get; set; }
        public string RCNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string EngineNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string HolderName { get; set; }
    }
}
