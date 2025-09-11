using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace LTI.Models
{
    public class Claim
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClaimID { get; set; }
        [Required]
        public int UserPolicyID { get; set; }
        [Required]
        [MaxLength(50)]
        public string ClaimType { get; set; }
        public string Reason { get; set; }
        [Required]
        public decimal ClaimedAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }
        [Required, MaxLength(15)]
        public string Status { get; set; } = "Submitted";
        [Required]
        public DateTime ClaimDate { get; set; } = DateTime.UtcNow;
        // navigation
       
    }

}