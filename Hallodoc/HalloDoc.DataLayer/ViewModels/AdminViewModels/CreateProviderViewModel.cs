using HalloDoc.DataLayer.Models;
using HalloDoc.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class CreateProviderViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Role> Roles { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MedicalLicense { get; set; }
        public string NPINumber { get; set; }
        public List<Region> Regions { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public string SelectedRegion { get; set; }
        public List<ProviderSelectedRegions> State { get; set; }
        public string Zip { get; set; }
        public string AltPhoneNumber { get; set; }
        public string BusinessName { get; set; }
        public string BusinessWebsite { get; set; }
        public string AdminNotes { get; set; }
        public string IsPhoto { get; set; }
        public IFormFile? Photo { get; set; }
        public IFormFile? ICA { get; set; }
        public IFormFile? BackgroundCheck { get; set; }
        public IFormFile? HIPAACompliance { get; set; }
        public IFormFile? NonDisclosureAgreement { get; set; }
        public bool IsICA { get; set; }
        public bool IsBackgroundCheck { get; set; }
        public bool IsHIPAACompliance { get; set; }
        public bool IsNonDisclosureAgreement { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public string PhotoName { get; set; }
        public bool check { get; set; }
        public int PhysicianId { get; set; }
        public int Status { get; set; }

    }
}
