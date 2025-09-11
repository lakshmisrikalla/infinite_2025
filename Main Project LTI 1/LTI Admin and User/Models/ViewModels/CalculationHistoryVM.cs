using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LTI.Models.ViewModels;

namespace LTI.Models.ViewModels
{
    public class CalculationHistoryVM
    {
        public int CalculationID { get; set; }
        public string PolicyType { get; set; }
        public string VehicleModel { get; set; }
        public int? VehicleAge { get; set; }
        public string TravelDetails { get; set; }
        //User Info
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal EstimatedPremium { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
    public class PolicyHistoryVM
    {
        public int UserPolicyID { get; set; }
        public string PolicyName { get; set; }
        public string InsuranceType { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class RenewalHistoryVM
    {
        public int RenewalID { get; set; }
        public int UserPolicyID { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
    public class ClaimHistoryVM
    {
        public int ClaimID { get; set; }
        public int UserPolicyID { get; set; }
        public string ClaimType { get; set; }
        public string Reason { get; set; }
        public decimal ClaimedAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public string Status { get; set; }
        public DateTime ClaimDate { get; set; }
    }
    public class UserHistoryVM
    {
        public List<CalculationHistoryVM> Calculations { get; set; }
        public List<PolicyHistoryVM> Policies { get; set; }
        public List<RenewalHistoryVM> Renewals { get; set; }
        public List<ClaimHistoryVM> Claims { get; set; }
    }
}