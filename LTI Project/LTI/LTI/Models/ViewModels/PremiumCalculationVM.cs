using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LTI.Models.ViewModels
{
    //public class PremiumCalculationVM
    //{
    //    [Required]
    //    public int UserID { get; set; }
    //    [Required]
    //    public int PlanID { get; set; }
    //    //Motor-Specific
    //    public string VehicleModel { get; set; }
    //    public int? VehicleAge { get; set; }
    //    //Travel-Specific
    //    public string TravelDetails { get; set; }
    //    public int? TripDurationDetails { get; set; }
    //    public int? TravellerAge { get; set; }
    //    public List<Policy> AvailablePlans { get; set; } // Add this
    //}

    public class PremiumCalculationVM
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public int ClientID { get; set; } // NEW

        [Required]
        public int PlanID { get; set; }

        // Motor-Specific
        public string VehicleModel { get; set; }
        public int? VehicleAge { get; set; }

        // Travel-Specific
        public string TravelDetails { get; set; }
        public int? TripDurationDetails { get; set; }
        public int? TravellerAge { get; set; }

        public List<Policy> AvailablePlans { get; set; }
    }

}