using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystemDAL.Models.Dto
{
    public class CustomerByIdDTO
    {
        public int? customer_id { get; set; }
        public string? customer_name { get; set; }
        public string? customer_location { get; set; }
        public string? customer_email { get; set; }
        public string? profileImgPath { get; set; }
        public IFormFile? File { get; set; }
    }
}
