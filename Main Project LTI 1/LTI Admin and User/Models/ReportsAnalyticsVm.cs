using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class ReportsAnalyticsVm
    {
        public string SelectedPeriod { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int BlockedUsers { get; set; }

        public int TotalClients { get; set; }
        public int ApprovedClients { get; set; }
        public int PendingClients { get; set; }
        public int BlockedClients { get; set; }

        public int TotalPolicies { get; set; }
        public int ApprovedPolicies { get; set; }
        public int PendingPolicies { get; set; }
        public int RejectedPolicies { get; set; }

        public decimal TotalRevenue { get; set; }

        public int TotalClaims { get; set; }
        public int ApprovedClaims { get; set; }
        public int RejectedClaims { get; set; }
        public int UnderReviewClaims { get; set; }
        public int SettledClaims { get; set; }
        public int SubmittedClaims { get; set; }

        public List<string> RevenueMonthLabels { get; set; } = new List<string>();
        public List<decimal> RevenueMonthValues { get; set; } = new List<decimal>();
        public List<string> NewUsersMonthLabels { get; set; } = new List<string>();
        public List<int> NewUsersMonthValues { get; set; } = new List<int>();
        public List<string> ClientStatusLabels { get; set; } = new List<string>();
        public List<int> ClientStatusValues { get; set; } = new List<int>();
        public List<string> PolicyStatusLabels { get; set; } = new List<string>();
        public List<int> PolicyStatusValues { get; set; } = new List<int>();
        public List<string> ClaimStatusLabels { get; set; } = new List<string>();
        public List<int> ClaimStatusValues { get; set; } = new List<int>();
    }
}


