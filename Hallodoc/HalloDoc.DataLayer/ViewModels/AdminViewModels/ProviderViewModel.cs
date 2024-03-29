using HalloDoc.Models;
using Hallodoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class ProviderViewModel
    {
        public List<Physician> PhysicianList { get; set; }
        public int PhysicianId { get; set; }
        public int RegionId { get; set; }
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public List<Region> Regions { get; set; }
    }
}
