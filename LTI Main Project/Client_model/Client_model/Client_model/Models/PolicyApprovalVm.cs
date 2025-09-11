namespace Client_model.Models
{
    public class PolicyApprovalVm
    {
        public int UserPolicyID { get; set; }
        public string Policy { get; set; }
        public string Holder { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public System.DateTime? StartDate { get; set; }
        public System.DateTime? EndDate { get; set; }
        public int? ApprovalID { get; set; }
        public string Decision { get; set; }
        public string Notes { get; set; }
    }
}
