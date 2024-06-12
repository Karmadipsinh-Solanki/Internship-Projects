using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystemDAL.Models.Dto
{
    public class LoginRequestDTO
    {
        public string user_email { get; set; }
        public string password { get; set; }
    }
}
