using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Models
{
    //checks whether we have provided input to the username and password fields
    public class PatientReqViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public required string LastName{ get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        public required string DOB { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "PinCode is required")]
        public required string PinCode { get; set; }

        [Required(ErrorMessage = "Street is required")]
        public required string Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        public required string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        public required string State { get; set; }

        [Required(ErrorMessage = "ZipCode is required")]
        public required string ZipCode { get; set; }

        [Required(ErrorMessage = "Fileurl is required")]
        public required string Fileurl { get; set; }
    }
}
