using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class ProviderInformationViewModel
    {
        public int PhysicianId { get; set; }
        public string? ProviderName { get; set; }
        public string? ProviderEmail { get; set; }
        public bool? StopNotification { get; set; }
        public string? Role { get; set; }
        public string? OnCallStatus { get; set; }
        public string? Status { get; set; }
    }
}
