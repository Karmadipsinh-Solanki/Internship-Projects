using System.ComponentModel.DataAnnotations;
using Hallodoc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Models
{
    // Define your view model
    public class RequestViewModelPatient
    {
        public string ?Symptoms { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string ?FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public  string ?LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        public  DateOnly DOB { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public  string ?Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public  string ?PhoneNumber { get; set; }

        [Required(ErrorMessage = "Street is required")]
        public  string ?Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        public  string ?City { get; set; }

        [Required(ErrorMessage = "State is required")]
        public  string ?State { get; set; }

        [Required(ErrorMessage = "ZipCode is required")]
        public  string ?ZipCode { get; set; }
        public  string ?Room { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string ?Password { get; set; }
        public bool ?isPassword { get; set; } = false;
        public IFormFile File { get; set; }


        


        [Required(ErrorMessage = "First name is required")]
        public string FFirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public required string FLastName { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public required string FPhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public required string FEmail { get; set; }

        [Required(ErrorMessage = "This is required")]
        public required string PasswordHash { get; set; }

        [Required(ErrorMessage = "This phield is required")]
        public required string RequirePasswordHash { get; set; }

        [Required(ErrorMessage = "Relation name is required")]
        public required string Relation { get; set; }

    }
}
