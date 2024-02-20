using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Models
{
    //checks whether we have provided input to the username and password fields
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public required string Username { get; set; }
    }
}
