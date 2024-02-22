using Hallodoc.Models;
using Hallodoc.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HalloDoc.Models;

public class FileUpload
{
    public IFormFile? File { get; set; }
    public int RequestId { get; set; }
    public string FileName { get; set; }
    public DateTime CreatedDate { get; set; }
}