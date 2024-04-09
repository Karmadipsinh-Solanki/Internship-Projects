using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public partial class ShiftDetailData
    {
        public int ShiftDetailId { get; set; }

        public string PhysicianName { get; set; }

        public string ShiftDate { get; set; }

        public string Region { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
    }
}
