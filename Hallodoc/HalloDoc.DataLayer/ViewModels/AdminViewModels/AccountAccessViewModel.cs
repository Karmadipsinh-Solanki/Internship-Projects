using HalloDoc.Models;
using Hallodoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class AccountAccessViewModel
    {
        public List<Role> Query { get; set; }
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
    }
}
