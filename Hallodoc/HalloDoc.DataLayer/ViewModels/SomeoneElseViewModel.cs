

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace HalloDoc.DataLayer.ViewModels;

public class SomeoneElseViewModel
{
    public string? Symptoms { get; set; }
    [Required(ErrorMessage = "Please enter this field")]
    public string? FirstName { get; set; }
    [Required(ErrorMessage = "Please enter this field")]
    public string? LastName { get; set; }
    [Required(ErrorMessage = "Please enter this field")]
    public DateTime DOB { get; set; }
    [Required(ErrorMessage = "Please enter this field")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Please enter this field")]
    public string? PhoneNumber { get; set; }
    [Required(ErrorMessage = "Please enter this field")]
    public string? Street { get; set; }
    [Required(ErrorMessage = "Please enter this field")]
    public string? City { get; set; }
    [Required(ErrorMessage = "Please enter this field")]
    public string? State { get; set; }
    [Required(ErrorMessage = "Please enter this field")]
    public string? ZipCode { get; set; }
    [Required(ErrorMessage = "Please enter this field")]
    public string? Room { get; set; }
    [Required(ErrorMessage = "Please enter this field")]
    public IFormFile File { get; set; }
    [Required(ErrorMessage = "Please enter this field")]
    public string? Relation { get; set; }
}

