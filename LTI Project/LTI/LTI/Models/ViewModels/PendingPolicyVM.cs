using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models.ViewModels
{
    public class PendingPolicyVM
    {
        public int UserPolicyID { get; set; }

        public string PolicyName { get; set; }

        public string InsuranceType { get; set; }

        public string Status { get; set; }

        public string PaymentStatus { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal BasePremium { get; set; } // from Policies table

    }
}