using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystemDAL.Models
{
    public class User
    {
        public string user_email { get; set; }
        public int user_id { get; set; }
        public int password { get; set; }
    }
}
