using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class VendorTable
    {
        public string ProfessionName { get; set; }
        public string BusinessName { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[+][0-9]*$", ErrorMessage = "Only valid numbers are allowed in this field.")]
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? FaxNumber { get; set; }
        public string? BusinessContact { get; set; }
        public int VendorId { get; set; }
    }
}
