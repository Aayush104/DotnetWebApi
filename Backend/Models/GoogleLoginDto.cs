using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class GoogleLoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        
        public bool EmailConfirmed { get; set; }
    }
}
