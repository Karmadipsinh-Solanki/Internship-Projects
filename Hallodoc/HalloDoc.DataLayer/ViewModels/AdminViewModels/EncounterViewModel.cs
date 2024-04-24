using HalloDoc.Models;
using Hallodoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class EncounterViewModel
    {
        public AdminNavbarViewModel adminNavbarViewModel { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string Location { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public DateTime DOB { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public DateTime? Date { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[+][0-9]*$", ErrorMessage = "Only valid numbers are allowed in this field.")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? HistoryOfPresentIllness { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? MedicalHistory { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? Medications { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? Allergies { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public decimal? temp { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public decimal? HR { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public decimal? RR { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public int? BPSystolic { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public int? BPDiastolic { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public decimal? O2 { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? Pain { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? Heent { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? CV { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? Chest { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? ABD { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? Extr { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? Skin { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? Neuro { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? Other { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? Diagnosis { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? TreatmentPlan { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? MedicationsDispensed { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? Procedures { get; set; }
        [Required(ErrorMessage = "Please enter this field")]
        public string? Followup { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string SelectedRegion { get; set; }
        public List<AdminSelectedRegions> State { get; set; }
        public int? RequestId { get; set; }
    }
}
