using Hallodoc;
using Hallodoc.Models.Models;
using HalloDoc.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HalloDoc.Models;
public class AdminDashboardTableView
{
    public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
    public int new_count { get; set; }
    public int pending_count { get; set; }
    public int active_count { get; set; }
    public int conclude_count { get; set; }
    public int toclose_count { get; set; }
    public int unpaid_count { get; set; }
    //public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
    public List<Request> requests { get; set; } = new List<Request>();
    public IQueryable<Request> query_requests { get; set; }
    public List<Region> regions { get; set; } = new List<Region>();
    public string? region { get; set; }
    public List<Physician> physicians { get; set; } = new List<Physician>();
    public string? physician { get; set; }
    public string status { get; set; }

    public List<CaseTag>? caseTags { get; set; }
    [Required(ErrorMessage = "Please enter the First Name")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Please enter the Last Name")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Phone Number is required")]
    [RegularExpression(@"^[+][0-9]*$", ErrorMessage = "Only valid numbers are allowed in this field.")]
    public string PhoneNo { get; set; }
    [Required(ErrorMessage = "Please enter the email address")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
    public string Email { get; set; }
    public string Message { get; set; }
    [Required(ErrorMessage = "Please enter Description")]
    public string Description { get; set; }
    public int PhysicianId { get; set; }
    public int RequestId { get; set; }
    [Required(ErrorMessage = "Please enter Description")]
    public string CancelDescription { get; set; }
    public string CaseTagId { get; set; }
    [Required(ErrorMessage = "Please enter Reason")]
    public string BlockReason { get; set; }
    public string Requestor { get; set; }//for sendagreement
    public int RequestTypeId { get; set; }//for sendagreement
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }

    //download filtered excel
    public string? search { get; set; }
    public string? requestor { get; set; }
    public int? RegionId { get; set; }


}