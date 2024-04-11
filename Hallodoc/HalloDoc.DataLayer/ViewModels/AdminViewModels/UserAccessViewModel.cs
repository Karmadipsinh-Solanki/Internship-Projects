using HalloDoc.DataLayer.Models;
using HalloDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class UserAccessViewModel
    {
        public List<Admin> Admins { get; set; }
        public List<Physician> Physicians { get; set; }
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public string AccountType { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}
