// Path: Models/PolicyCreateVm.cs
using System.ComponentModel.DataAnnotations;

namespace Client_model.Models
{
    public class PolicyCreateVm
    {
        public int PolicyID { get; set; }
        [Required, StringLength(120)]
        public string PolicyName { get; set; }

        [Display(Name = "Policy Type")]
        public int PolicyTypeID { get; set; }

        [Display(Name = "Plan Kind")]
        public string PlanKind { get; set; } // "ThirdParty" or "Comprehensive"

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        public string CoverageDetails { get; set; }

        [Required, Range(1, 120)]
        public int DurationMonths { get; set; }

        [Required, Range(0.0, 100000000.0)]
        [Display(Name = "Base Premium")]
        public decimal BasePremium { get; set; }
        [StringLength(50)]
        public string Status { get; set; }

        public bool IsActive { get; set; }
    }
}
