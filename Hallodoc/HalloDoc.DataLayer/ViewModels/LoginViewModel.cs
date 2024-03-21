using System.ComponentModel.DataAnnotations;

namespace HalloDoc.DataLayer.ViewModels
{
    //checks whether we have provided input to the username and password fields
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter the user's email address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public required string PasswordHash { get; set; }
    }
}
