
using System.ComponentModel.DataAnnotations;
namespace MediClinic.Models.ViewModel { 

public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
