using Hallodoc;
using Hallodoc.Models.Models;
using HalloDoc.DataLayer.Models;
using HalloDoc.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class ViewUploadViewModel
    {
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public string? patient_name { get; set; }
        public string? name { get; set; }
        public string? confirmation_number { get; set; }
        public List<RequestWiseFile>? requestWiseFiles { get; set; }
        public int? RequestId { get; set; }
        public IFormFile? File { get; set; }
        //public partialViewModel partialViewModel { get; set; }
        public DateOnly? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set;}

        //for close case
        [Required(ErrorMessage = "Please enter FirstName")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Please enter LastName")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Please enter DOB")]
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Please enter the patient's phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the patient's email address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string Email { get; set; }

    }
}
