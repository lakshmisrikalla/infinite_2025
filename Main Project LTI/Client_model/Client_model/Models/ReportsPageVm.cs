using System;
using System.Collections.Generic;

namespace Client_model.Models
{
    public class ReportsPageVm
    {
        // Filters
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? Year { get; set; }
        public int? Quarter { get; set; }

        // Summary totals
        public decimal TotalAmount { get; set; }          // Total payments collected
        public int TotalBuyers { get; set; }               // Unique policy holders count
        public int PoliciesSold { get; set; }               // Total policies sold
        public int ClaimsCount { get; set; }                // Total claims count
        public int RenewalsCount { get; set; }              // Total renewals count
        public int DueCount { get; set; }                    // Unpaid dues count

        // Lists for details
        public List<TopPolicyVm> TopPoliciesByAmount { get; set; } = new List<TopPolicyVm>();
        public List<TopPolicyVm> LowPoliciesByAmount { get; set; } = new List<TopPolicyVm>();
    }

    public class TopPolicyVm
    {
        public int PolicyID { get; set; }
        public string PolicyName { get; set; }
        public decimal TotalCollected { get; set; }
        public int BuyersCount { get; set; }
    }
}
