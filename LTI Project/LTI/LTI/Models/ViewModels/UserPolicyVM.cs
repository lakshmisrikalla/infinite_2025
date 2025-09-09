using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LTI.Models.ViewModels;
using System.ComponentModel.DataAnnotations;


namespace LTI.Models.ViewModels
{
    public class UserPolicyVM
    {
        public int UserPolicyID { get; set; }
        public int PolicyID { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}