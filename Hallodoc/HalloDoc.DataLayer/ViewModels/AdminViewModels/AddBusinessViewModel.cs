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
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Business Name is required")]
        public string BusinessName { get; set; }
        [Required(ErrorMessage = "Fax Number is required")]
        public string FaxNumber { get; set; }

        //[RegularExpression(@"^[a-zA-Z]{1}[a-zA-Z0-9.]{1,}@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.(?:[a-zA-Z]{2,}|[a-zA-Z]{2,}\.[a-zA-Z]{2,})$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public int State { get; set; }
        public List<Region> States { get; set; }
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
