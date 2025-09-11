namespace Client_model.ViewModels
{
    public class PolicyHolderVm
    {
        public int UserPolicyID { get; set; }
        public string FullName { get; set; }
        public string Policy { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
    }
}
