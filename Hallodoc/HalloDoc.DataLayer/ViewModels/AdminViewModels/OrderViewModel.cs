using Hallodoc;
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
    public class OrderViewModel
    {
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public int RequestId { get; set; }
        public int VendorId { get; set; }
        [Required(ErrorMessage = "Please select profession")]
        public string? Profession { get; set; }
        [Required(ErrorMessage = "Please select business")]
        public string? BusinessInProfession { get; set; }
        [Required(ErrorMessage = "Please enter the business contact")]
        public string? BusinessContact { get; set; }
        [Required(ErrorMessage = "Please enter the email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Please enter the fax number")]
        public string? FaxNumber { get; set; }
        [Required(ErrorMessage = "Please enter prescription")]
        public string? Prescription { get; set; }
        [Required(ErrorMessage = "Please enter number of refill")]
        public int? NoOfRefill{ get; set; }
        public List<HealthProfessionalType>? healthProfessionalTypes { get; set; }
        public List<HealthProfessional>? healthProfessionals { get; set; }
    }
}
