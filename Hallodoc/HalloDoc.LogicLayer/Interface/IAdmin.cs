using Hallodoc;
using HalloDoc.DataLayer.ViewModels.AdminViewModels;
using HalloDoc.Models;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Interface
{
    public interface IAdmin
    {
        public AdminDashboardTableView adminDashboard(string status,string? search, int? region, string? requestor);
        public int createRequest(CreateRequestViewModel model);
        public bool sendLink(AdminDashboardTableView model);
        public MemoryStream downloadExcel();
        public ViewCaseModel viewCase(int id);
        public bool viewCase(ViewCaseModel model);
        public ViewNotesViewModel viewNotes(int id);
        public bool viewNotes(ViewNotesViewModel model);
        public ViewUploadViewModel viewUpload(int id);
        public bool viewUpload(ViewUploadViewModel model);
        public bool SendOrder(OrderViewModel model);
        public OrderViewModel SendOrder(int id);
        //public bool SendOrder(OrderViewModel model);
        public List<Region> fetchRegions();
        public List<Physician> fetchPhysicians(int id);
        public List<CaseTag> fetchTags();
        public List<HealthProfessional> fetchBusinessDetail();
        public bool assignCase(AdminDashboardTableView model);
        public bool cancelCase(AdminDashboardTableView model);
        public bool clearCase(AdminDashboardTableView model);
        public bool blockCase(AdminDashboardTableView model);
    }
}
