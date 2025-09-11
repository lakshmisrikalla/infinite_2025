using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class UserDetailsViewModel
    {
        public int LoginID { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string UserStatus { get; set; }
        public DateTime? UserCreatedAt { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LoginCreatedAt { get; set; }

        public List<UserPolicyRow> Policies { get; set; } = new List<UserPolicyRow>();
        public List<PaymentRow> Payments { get; set; } = new List<PaymentRow>();
        public List<RenewalRow> Renewals { get; set; } = new List<RenewalRow>();
        public List<ClaimRow> Claims { get; set; } = new List<ClaimRow>();
        public List<DocumentRow> Documents { get; set; } = new List<DocumentRow>();
    }

    public class UserPolicyRow
    {
        public int UserPolicyID { get; set; }
        public int PolicyID { get; set; }
        public string PolicyName { get; set; }
        public string PolicyType { get; set; }
        public string ClientName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class PaymentRow
    {
        public int PaymentID { get; set; }
        public int UserPolicyID { get; set; }
        public decimal Amount { get; set; }
        public string Mode { get; set; }
        public string Status { get; set; }
        public DateTime PaidAt { get; set; }
        public string GatewayRef { get; set; }
    }

    public class RenewalRow
    {
        public int RenewalID { get; set; }
        public int UserPolicyID { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }

    public class ClaimRow
    {
        public int ClaimID { get; set; }
        public int UserPolicyID { get; set; }
        public DateTime ClaimDate { get; set; }
        public string ClaimType { get; set; }
        public string Reason { get; set; }
        public decimal ClaimedAmount { get; set; }
        public string Status { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public DateTime? DecisionAt { get; set; }
    }

    public class DocumentRow
    {
        public int DocumentID { get; set; }
        public string OwnerType { get; set; }
        public int OwnerID { get; set; }
        public string DocumentType { get; set; }
        public string FilePath { get; set; }
        public bool IsVerified { get; set; }
        public string VerifiedByRole { get; set; }
        public int? VerifiedByID { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string Visibility { get; set; }
    }
}

