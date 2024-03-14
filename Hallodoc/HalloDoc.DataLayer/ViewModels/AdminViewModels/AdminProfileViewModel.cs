using Hallodoc;
using HalloDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class AdminProfileViewModel
    {
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Status { get; set; }
        public List<Role>? role { get; set; }
        //public string? Role { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ConfirmEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        
    }
}
