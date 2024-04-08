using Hallodoc;
using HalloDoc.DataLayer.Models;
using HalloDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class ProviderOnCallViewModel
    {
        public List<PhysicianProfile> MDOnCall { get; set; }
        public List<PhysicianProfile> PhysicianOffDuty { get; set; }
        public List<Region> Regions { get; set; }
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
    }


}
