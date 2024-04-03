using Hallodoc;
using Hallodoc.Models.Models;
using HalloDoc.DataLayer.Models;
using HalloDoc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class ViewCaseModel
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

        [Required(ErrorMessage = "Please enter the patient's phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the patient's email address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string Email { get; set; }
        //BusinessName/Address
        public string BusinessAddress { get; set; }
        public string Room { get; set; }
        public int RequestId { get; set; }
        public int RequestClientId { get; set; }
        public int RequestTypeId { get; set; }
        public string Status { get; set; }
        public string ConfirmationNo { get; set; }
        public string PatientNotes { get; set; }
        public string Requestor { get; set; }
        public string? Admin_notes { get; set; }
        public int PhysicianId { get; set; }
        [Required(ErrorMessage = "Please enter the Description")]
        public string Description { get; set; }
        ////Assign case
        public List<Region> regions { get; set; } = new List<Region>();
        public string? region { get; set; }
        public List<Physician> physicians { get; set; } = new List<Physician>();
        public string? physician { get; set; }
    }
}

