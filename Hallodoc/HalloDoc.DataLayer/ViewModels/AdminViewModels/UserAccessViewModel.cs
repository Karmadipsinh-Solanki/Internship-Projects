using HalloDoc.DataLayer.Models;
using HalloDoc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class UserAccessViewModel
    {
        public int Account_Type { get; set; }
        public string AccountPOC { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[+][0-9]*$", ErrorMessage = "Only valid numbers are allowed in this field.")]
        public string Phone { get; set; }
        public int Status { get; set; }
        public int OpenRequests { get; set; }
        public int Id { get; set; }
    }
}
