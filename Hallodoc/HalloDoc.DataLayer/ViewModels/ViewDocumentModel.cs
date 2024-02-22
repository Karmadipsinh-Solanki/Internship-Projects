using Hallodoc.Models.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HalloDoc.DataLayer.ViewModels;

public class ViewDocumentModel
{
    public string? patient_name { get; set; }
    public string? uploader_name { get; set; }
    public string? name { get; set; }
    public string? confirmation_number { get; set; }
    public List<RequestWiseFile>? requestWiseFiles { get; set; }
    public int? RequestId { get; set; }
    public IFormFile? File { get; set; }
    public partialViewModel partialViewModel { get; set; }
    public DateTime? CreatedDate { get; set; }
}