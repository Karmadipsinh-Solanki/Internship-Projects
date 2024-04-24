using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class SearchRecordTable
    {
        public int RequestId { get; set; }
        public string? PatientNote { get; set; }
        public string? AdminNote { get; set; }
        public string CancelledByProviderNote { get; set; }
        public string PhysicianNote { get; set; }
        public string Physician { get; set; }
        public int RequestStatus { get; set; }
        public string Zip { get; set; }
        public string Address { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[+][0-9]*$", ErrorMessage = "Only valid numbers are allowed in this field.")]
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? CloseCaseDate { get; set; }
        public DateTime? DateOfService { get; set; }
        public int? Requestor { get; set; }
        public string PatientName { get; set; }
    }
}
