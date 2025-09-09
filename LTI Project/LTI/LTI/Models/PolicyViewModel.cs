using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class PolicyViewModel
    {
        public int PolicyID { get; set; }
        public string PolicyName { get; set; }
        public string PlanKind { get; set; }
        public string Description { get; set; }
        public int DurationMonths { get; set; }
        public decimal BasePremium { get; set; }
        public string TypeName { get; set; }
        public string CompanyName { get; set; }

        public string Status { get; set; }

    }

    public class OverviewViewModel
    {
        public List<PolicyViewModel> LatestPolicies { get; set; }
        public int TotalPolicies { get; set; }
        public int TotalUsers { get; set; }
    }

}