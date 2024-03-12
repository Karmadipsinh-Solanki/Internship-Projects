using Hallodoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class OrderViewModel
    {
        public int RequestId { get; set; }
        public int VendorId { get; set; }
        public string? Profession { get; set; }
        public string? BusinessInProfession { get; set; }
        public string? BusinessContact { get; set; }
        public string? Email { get; set; }
        public string? FaxNumber { get; set; }
        public string? Prescription { get; set; }
        public int? NoOfRefill{ get; set; }
        public List<HealthProfessionalType>? healthProfessionalTypes { get; set; }
        public List<HealthProfessional>? healthProfessionals { get; set; }
    }
}
