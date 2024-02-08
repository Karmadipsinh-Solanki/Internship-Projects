using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Models
{
    //checks whether we have provided input to the username and password fields
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public required string PasswordHash { get; set; }
    }
}
