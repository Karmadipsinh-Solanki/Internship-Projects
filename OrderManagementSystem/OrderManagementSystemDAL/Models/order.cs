using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystemDAL.Models
{
    public class order
    {
        public int order_id { get; set; }
        [Required(ErrorMessage = "Product Name is required.")]
        public string product_name { get; set; }
        public int customer_id { get; set; }
        [Required(ErrorMessage = "Customer Name is required.")]
        public string customer_name { get; set; }
        [Required(ErrorMessage = "Quantity is required.")]
        public int quantity { get; set; }
        [Required(ErrorMessage = "Order Amount is required.")]
        public int order_amount { get; set; }
        public int total_amount { get; set; }
        public List<customer> customer { get; set; }
    }
}
