using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LTI.Models
{
    public class UserPolicy
    {
        public int UserPolicyID { get; set; }
        public int UserID { get; set; }
        public int PolicyID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Pending";     // Pending/Active/Expired
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid/Paid
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [MaxLength(12)]
        public string InsuranceType { get; set; }
        [MaxLength(100)]
        public string NomineeName { get; set; }
        //navigation
        public virtual User User { get; set; }
        public virtual Policy Policy { get; set; }
    }
}