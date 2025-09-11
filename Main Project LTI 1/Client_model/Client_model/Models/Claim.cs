namespace Client_model.Models
{
    using System;
    public partial class Claim
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

        public virtual UserPolicy UserPolicy { get; set; }
    }
}
