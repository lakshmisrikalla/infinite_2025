using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LTI.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        [ForeignKey("LoginCredentials")]
        public int LoginID { get; set; }

        [Required]
        public string FullName { get; set; }
        [NotMapped]
        //[Required]
        public string Username { get; set; }
        [NotMapped]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //navigation
        //navigation
        public virtual LoginCredentials LoginCredentials { get; set; }
        public virtual ICollection<PremiumCalculation> PremiumCalculations { get; set; }
      
    }
}
