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
        public int Account_Type { get; set; }
        public string AccountPOC { get; set; }
        public string Phone { get; set; }
        public int Status { get; set; }
        public int OpenRequests { get; set; }
        public int Id { get; set; }
    }
}
