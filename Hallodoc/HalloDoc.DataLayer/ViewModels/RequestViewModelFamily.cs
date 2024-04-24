using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.DataLayer.ViewModels
{
    public class RequestViewModelFamily
    {
        public string Symptoms { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        public required DateOnly DOB { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[+][0-9]*$", ErrorMessage = "Only valid numbers are allowed in this field.")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Street is required")]
        public required string Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        public required string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        public required string State { get; set; }

        [Required(ErrorMessage = "Zip Code is required")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Only numbers are allowed in this field.")]
        public required string FZipCode { get; set; }
        public required string Room { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        public bool isPassword { get; set; } = false;
        public IFormFile File { get; set; }


        [Required(ErrorMessage = "First name is required")]
        public string FFirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public required string FLastName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[+][0-9]*$", ErrorMessage = "Only valid numbers are allowed in this field.")]
        public required string FPhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public required string FEmail { get; set; }

        [Required(ErrorMessage = "Relation name is required")]
        public required string Relation { get; set; }

    }
}
