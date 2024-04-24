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

    public class AddBusinessViewModel
    {
        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[+][0-9]*$", ErrorMessage = "Only valid numbers are allowed in this field.")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Business Name is required")]
        public string BusinessName { get; set; }
        [Required(ErrorMessage = "Fax Number is required")]
        public string FaxNumber { get; set; }

        [RegularExpression(@"^[a-zA-Z]{1}[a-zA-Z0-9.]{1,}@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.(?:[a-zA-Z]{2,}|[a-zA-Z]{2,}\.[a-zA-Z]{2,})$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Street { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string City { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public int State { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public List<Region> States { get; set; }
        [Required(ErrorMessage = "Zip Code is required")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Only numbers are allowed in this field.")]
        public string Zipcode { get; set; }
        public string Room { get; set; }
        public string BusinessContact { get; set; }
        public int ProfessionType { get; set; }
        public List<HealthProfessionalType> ProfessionTypes { get; set; }
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public int healthProfessionId { get; set; }
        public string Header { get; set; }
    }

}
