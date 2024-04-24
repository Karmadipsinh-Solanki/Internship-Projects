using HalloDoc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{

    public class SearchRecordsViewModel
    {
        public List<SearchRecordTable> Query { get; set; }
        public List<SearchRecordTable> ExcelData { get; set; }
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public int? RequestStatus { get; set; }
        public string? PatientName { get; set; }
        public int? RequestType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? ProviderName { get; set; }
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[+][0-9]*$", ErrorMessage = "Only valid numbers are allowed in this field.")]
        public string? PhoneNumber { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}
