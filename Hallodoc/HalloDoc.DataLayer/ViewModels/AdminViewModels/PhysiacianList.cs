using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class PhysicianList
    {
        public bool Notification { get; set; }
        public string ProviderName { get; set; }
        public string RoleName { get; set; }
        public int OnCallStatus { get; set; }
        public int Status { get; set; }
        public int PhysicianId { get; set; }
    }
}
