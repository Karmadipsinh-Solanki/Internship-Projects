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
    public class ProviderShift
    {
        public List<PhysicianList> PhysicianList { get; set; }
        public List<Region> RegionList { get; set; }
        public List<Physician> Physicians { get; set; }
        public DateTime ShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsRepeat { get; set; }
        public string RepeatDays { get; set; }
        public int ShiftDetailId { get; set; }
        public int RepeatEnd { get; set; }
        public int PhysicianId { get; set; }
        public int RegionId { get; set; }
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}
