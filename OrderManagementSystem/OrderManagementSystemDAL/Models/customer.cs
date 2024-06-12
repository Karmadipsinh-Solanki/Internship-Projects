using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystemDAL.Models
{
    public class customer
    {
        public int? customer_id {  get; set; }
        public string? profileImgPath {  get; set; }

        public IFormFile? File { get; set; }
        [Required(ErrorMessage = "Customer name is required.")]
        [RegularExpression(@"^[^\d]+$", ErrorMessage = "Customer name cannot contain numbers.")]
        public string? customer_name { get; set; }

        [Required(ErrorMessage = "Customer location is required.")]
        public string? customer_location { get; set; }

        [Required(ErrorMessage = "Customer email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? customer_email { get; set; }
        public bool? isDeleted { get; set; }

    }
}
