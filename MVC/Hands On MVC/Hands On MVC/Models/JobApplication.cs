using System;
using System.ComponentModel.DataAnnotations;

namespace Hands_On_MVC.Models
{
    public class JobApplication
    {
        [Required(ErrorMessage = "Date of Birth is required")]
        [DOBRange(ErrorMessage = "Age must be between 21 and 25 years")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Date of Joining is required")]
        [DOJValid(ErrorMessage = "Date of Joining cannot be in the future")]
        public DateTime DOJ { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [PasswordFormat(ErrorMessage = "Password must start with uppercase, followed by a digit, then 5 characters")]
        public string Password { get; set; }
    }
}

