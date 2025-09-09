using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class Policy
    {
        public int PolicyID { get; set; }
        public int ClientID { get; set; }
        public int PolicyTypeID { get; set; }
        public string PolicyName { get; set; }
        public string PlanKind { get; set; }
        public string Description { get; set; }
        public string CoverageDetails { get; set; }
        public int DurationMonths { get; set; }
        public decimal BasePremium { get; set; }
        public string Status { get; set; }
    }
}