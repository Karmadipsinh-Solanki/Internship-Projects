using System.ComponentModel.DataAnnotations;

namespace Hallodoc.Models.ViewModels
{
    public class EditProfileViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DOB { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string PasswordHash { get; set; }
        public partialViewModel partialViewModel { get; set; }
    }
}
