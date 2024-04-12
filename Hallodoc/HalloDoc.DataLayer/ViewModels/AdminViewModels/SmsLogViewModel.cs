using HalloDoc.DataLayer.Models;
using HalloDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class SmsLogViewModel
    {
        //public List<BlockRequest> Query { get; set; }
        public List<SmsLogTable> Query { get; set; }
        public int PatientId { get; set; }
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public string ReceiverName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime SentDate { get; set; }
        public string PhoneNumber { get; set; }
        public List<AspNetRole> Roles { get; set; }
        public int Role { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}
