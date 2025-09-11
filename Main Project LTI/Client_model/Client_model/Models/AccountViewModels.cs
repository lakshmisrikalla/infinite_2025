using System.ComponentModel.DataAnnotations;

namespace Client_model.Models
{
    public class RegisterClientVm
    {
        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; }

        [Required, StringLength(150)]
        public string CompanyName { get; set; }

        [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        // NEW: Contact phone for client company
        [Phone, StringLength(20)]
        public string ContactPhone { get; set; }
    }

    public class LoginVm
    {
        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, StringLength(16)]
        public string CompanyCode { get; set; }
    }

    public class ForgotVm
    {
        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; }

        [Required, StringLength(150)]
        public string CompanyName { get; set; }
    }

    public class ChangePasswordVm
    {
        [Required, DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password), Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }

    // Profile edit VM
    public class ProfileEditVm
    {
        [Required, StringLength(150)]
        public string CompanyName { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string ContactEmail { get; set; }

        [Phone, StringLength(20)]
        public string ContactPhone { get; set; }

        // Not editing company code here
    }
}
