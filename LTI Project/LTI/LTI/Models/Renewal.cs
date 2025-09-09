using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LTI.Models
{
    public class Renewal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RenewalID { get; set; }
        [Required]
        public int UserPolicyID { get; set; }
        [Required]
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required, MaxLength(12)]
        public string Status { get; set; } = "Pending";
        // navigation
        public virtual UserPolicy UserPolicy { get; set; }
    }
}