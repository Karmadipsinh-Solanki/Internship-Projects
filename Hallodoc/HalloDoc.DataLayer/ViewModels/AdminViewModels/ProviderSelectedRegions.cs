using HalloDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class ProviderSelectedRegions
    {
        public int RegionId { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
    }
}
