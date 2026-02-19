using System.ComponentModel.DataAnnotations;

namespace MediClinic.Models
{
    public partial class ChemistResetPasswordViewModel
    {
        [Required(ErrorMessage = "Old password is required")]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? PasswordConfirmation { get; set; }
    }
}
