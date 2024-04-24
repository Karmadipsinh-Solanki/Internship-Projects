using Hallodoc.Models.Models;
using HalloDoc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class CreateRequestViewModel
    {
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        //in reqClent table
        public string Notes { get; set; }
        [Required]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[+][0-9]*$", ErrorMessage = "Only valid numbers are allowed in this field.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the patient's email address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string Email { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required(ErrorMessage = "Zip Code is required")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Only numbers are allowed in this field.")]
        public string ZipCode { get; set; }
        [Required]
        public string Room { get; set; }
        public string AdminNotes { get; set; }
        public int RequestId { get; set; }
        public int RequestClientId { get; set; }
        public int RequestTypeId { get; set; }
        public string Status { get; set; }
        public string ConfirmationNo { get; set; }
        public string Requestor { get; set; }
        public string? Admin_notes { get; set; }
    }
}