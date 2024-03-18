using HalloDoc.Models;
using Hallodoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class EncounterViewModel
    {
        public AdminNavbarViewModel adminNavbarViewModel { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }
        public DateTime DOB { get; set; }
        public DateTime? Date { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? HistoryOfPresentIllness { get; set; }
        public string? MedicalHistory { get; set; }
        public string? Medications { get; set; }
        public string? Allergies { get; set; }
        public decimal? temp { get; set; }
        public decimal? HR { get; set; }
        public decimal? RR { get; set; }
        public int? BPSystolic { get; set; }
        public int? BPDiastolic { get; set; }
        public decimal? O2 { get; set; }
        public string? Pain { get; set; }
        public string? Heent { get; set; }
        public string? CV { get; set; }
        public string? Chest { get; set; }
        public string? ABD { get; set; }
        public string? Extr { get; set; }
        public string? Skin { get; set; }
        public string? Neuro { get; set; }
        public string? Other { get; set; }
        public string? Diagnosis { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? MedicationsDispensed { get; set; }
        public string? Procedures { get; set; }
        public string? Followup { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string SelectedRegion { get; set; }
        public List<AdminSelectedRegions> State { get; set; }
        public int? RequestId { get; set; }
    }
}
