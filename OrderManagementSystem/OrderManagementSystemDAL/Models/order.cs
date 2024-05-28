using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystemDAL.Models
{
    public class order
    {
        public int order_id { get; set; }
        public string product_name { get; set; }
        public int customer_id { get; set; }
        public int quantity { get; set; }
        public int order_amount { get; set; }
    }
}
