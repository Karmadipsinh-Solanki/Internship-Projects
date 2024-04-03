using HalloDoc.Models;
using Hallodoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using HalloDoc.DataLayer.Models;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class CreateAdminViewModel
    {
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Please enter Password")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Please enter Roles")]
        public List<Role>? Roles { get; set; }
        public string? Role { get; set; }
        [Required(ErrorMessage = "Please enter FirstName")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Please enter LastName")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Please enter the patient's email address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
           ErrorMessage = "Please enter valid Email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Please Confirm Email")]
        public string? ConfirmEmail { get; set; }
        [Required(ErrorMessage = "Please enter the patient's phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? PhoneNumber1 { get; set; }
        [Required(ErrorMessage = "Please enter the patient's phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? PhoneNumber2 { get; set; }
        [Required(ErrorMessage = "Please enter Address")]
        public string? Address1 { get; set; }
        [Required(ErrorMessage = "Please enter Address")]
        public string? Address2 { get; set; }
        [Required(ErrorMessage = "Please enter city")]
        public string? city { get; set; }
        public int? StateId { get; set; }
        [Required(ErrorMessage = "Please enter zipcode")]
        public string? zip { get; set; }
        [Required(ErrorMessage = "Please enter State")]
        public List<Region>? regions { get; set; } = new List<Region>();
        public DateTime? CreatedDate { get; set; }
        public string SelectedRegion { get; set; }
        [Required(ErrorMessage = "Please enter State")]
        public List<AdminSelectedRegions> State { get; set; }
    }
}
