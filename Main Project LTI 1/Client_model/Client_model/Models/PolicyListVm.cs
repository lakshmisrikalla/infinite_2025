// Models/PolicyListVm.cs
using System;

namespace Client_model.Models
{
    public class PolicyListVm
    {
        public int PolicyID { get; set; }
        public string PolicyName { get; set; }
        public string PolicyTypeName { get; set; }
        public string PlanKind { get; set; }
        public decimal BasePremium { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CoverageDetails { get; set; }
        public string Description { get; set; }

        // optional latest terms doc info (if you store docs)
        public int? TermsDocumentID { get; set; }
        public string TermsFileName { get; set; }
    }
}
