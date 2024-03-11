using Hallodoc;
using Hallodoc.Models.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class ViewUploadViewModel
    {
        public string? patient_name { get; set; }
        public string? name { get; set; }
        public string? confirmation_number { get; set; }
        public List<RequestWiseFile>? requestWiseFiles { get; set; }
        public int? RequestId { get; set; }
        public IFormFile? File { get; set; }
        //public partialViewModel partialViewModel { get; set; }
        public DateOnly? CreatedDate { get; set; }
    }
}
