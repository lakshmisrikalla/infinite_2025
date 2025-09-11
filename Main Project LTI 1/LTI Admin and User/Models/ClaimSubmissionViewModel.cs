using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{

    public class ClaimSubmissionViewModel
    {
        public int UserPolicyID { get; set; }
        public string PolicyName { get; set; }
        public string PolicyType { get; set; }
        public string InsuranceType { get; set; }
        public decimal BasePremium { get; set; }

        public string ClaimType { get; set; }
        public string Reason { get; set; }
        public decimal ClaimedAmount { get; set; }
    }

}