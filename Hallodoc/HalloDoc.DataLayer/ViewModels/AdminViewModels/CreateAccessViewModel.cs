using HalloDoc.Models;
using Hallodoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc.DataLayer.Models;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class CreateAccessViewModel
    {
        public List<Menu> Query { get; set; }
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public string RoleName { get; set; }
        public string AccountType { get; set; }
        public string RoleId { get; set; }
    }
}
