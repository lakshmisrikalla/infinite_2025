using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LTI.Models
{
    public class PremiumCalculation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Identity column
        public int CalculationID { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserID { get; set; }

        [Required]
        [ForeignKey("PolicyType")]
        public int PolicyTypeID { get; set; }

        [MaxLength(100)]
        public string VehicleModel { get; set; }   // nullable

        public int? VehicleAge { get; set; }       // nullable

        public string TravelDetails { get; set; }  // nullable

        [Required]
        public decimal EstimatedPremium { get; set; }

        [Required]
        public DateTime CalculatedAt { get; set; }

        // Navigation properties (optional)
        public virtual User User { get; set; }
        public virtual PolicyType PolicyType { get; set; }
    }
}