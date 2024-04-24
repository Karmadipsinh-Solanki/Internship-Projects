using System.ComponentModel.DataAnnotations;

namespace HalloDoc.DataLayer.ViewModels
{
    public class EditProfileViewModel
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DOB { get; set; }
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[+][0-9]*$", ErrorMessage = "Only valid numbers are allowed in this field.")]
        public string? PhoneNumber { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [Required(ErrorMessage = "Zip Code is required")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Only numbers are allowed in this field.")]
        public string? ZipCode { get; set; }
        public string PasswordHash { get; set; }
        public partialViewModel partialViewModel { get; set; }
    }
}
