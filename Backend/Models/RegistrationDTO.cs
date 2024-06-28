using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class RegistrationDTO
    {
        [Required(ErrorMessage = "Username is required")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.")]
        public string Password { get; set; }
    }
}
