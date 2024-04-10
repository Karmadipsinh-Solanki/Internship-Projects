using HalloDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class EditAccessViewModel
    {
        public List<MenuItem> Query { get; set; }
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public string RoleName { get; set; }
        public string AccountType { get; set; }
        public string RoleId { get; set; }
        public int Id { get; set; }
    }
}
