using System;
using System.ComponentModel.DataAnnotations;

namespace LTI.Models
{
    public class LoginCredentials
    {
        [Key]
        public int LoginID { get; set; }

        [Required]
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
