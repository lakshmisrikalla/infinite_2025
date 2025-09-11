using System.Collections.Generic;

namespace Client_model.Models
{
    public class DashboardSummaryVm
    {
        public int TotalPolicies { get; set; }
        public int ActivePolicies { get; set; }
        public int PolicyHolders { get; set; }
        public int ClaimsCount { get; set; }
        public int PendingApprovals { get; set; }
        public int PendingRenewals { get; set; }
        public int DueSoon { get; set; }
        public decimal CollectedPayments { get; set; }
        public IEnumerable<SeriesPoint> RevenueByMonth { get; set; }
        public IEnumerable<SeriesPoint> TopPolicies { get; set; }
    }

    public class SeriesPoint
    {
        public string Label { get; set; }
        public decimal Value { get; set; }
        public string Icon { get; set; }
    }
}
