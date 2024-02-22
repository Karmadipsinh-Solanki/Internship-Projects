

using System;
using Microsoft.AspNetCore.Http;
namespace HalloDoc.DataLayer.ViewModels;

public class SomeoneElseViewModel
{
    public string? Symptoms { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DOB { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Room { get; set; }
    public IFormFile File { get; set; }
    public string? Relation { get; set; }
}

