namespace Client_model.Models
{
    public class PaymentVm
    {
        public int PaymentID { get; set; }
        public int UserPolicyID { get; set; }
        public string Holder { get; set; }     // user's FullName
        public string Policy { get; set; }     // policy name
        public decimal Amount { get; set; }
        public string Mode { get; set; }
        public string Status { get; set; }
        public System.DateTime PaidAt { get; set; }
        public string GatewayRef { get; set; }
    }
}
