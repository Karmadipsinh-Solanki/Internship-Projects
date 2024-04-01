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
        public AdminDashboardTableView adminDashboard(string status,string? search, int? region, string? requestor, int page = 1, int pageSize = 10);
        public int createRequest(CreateRequestViewModel model);
        public bool verifyState(CreateRequestViewModel model);
        public CreateRequestViewModel createRequest();
        public bool sendLink(AdminDashboardTableView model);
        public MemoryStream downloadExcel();
        public ViewCaseModel viewCase(int id);
        public bool viewCase(ViewCaseModel model);
        public bool viewCaseAssignModal(ViewCaseModel model);
        public ViewNotesViewModel viewNotes(int id);
        public bool viewNotes(ViewNotesViewModel model);
        public ViewUploadViewModel viewUpload(int id);
        public bool viewUpload(ViewUploadViewModel model);
        public bool closeCaseBtn(int requestId);
        public bool closeCaseSaveBtn(ViewUploadViewModel model);
        public ViewUploadViewModel closeCase(int id);
        public OrderViewModel SendOrder(int id);
        public bool SendOrder(OrderViewModel model);
        //public bool SendOrder(OrderViewModel model);
        public List<Region> fetchRegions();
        public List<Physician> fetchPhysicians(int id);
        public List<CaseTag> fetchTags();
        public List<HealthProfessional> fetchBusiness(int id);
        public HealthProfessional fetchBusinessDetail(int id);
        public bool assignCase(AdminDashboardTableView model);
        public bool transferCase(AdminDashboardTableView model);
        public bool cancelCase(AdminDashboardTableView model);
        public bool clearCase(AdminDashboardTableView model);
        public bool blockCase(AdminDashboardTableView model);
        public bool sendAgreement(AdminDashboardTableView model);
        public bool saveAdministratorDetail(AdminProfileViewModel model);
        public bool saveBillingInformation(AdminProfileViewModel model);
        public bool resetPassword(AdminProfileViewModel model);
        public AdminProfileViewModel adminProfile();
        public List<Region> fetchAdminRegions();
        public bool encounter(EncounterViewModel model);
        public EncounterViewModel encounter(int id);
        public bool mailDocument(List<int> requestFilesId, int requestId);
        public bool deleteViewUploadFile(string fileids);


        public CreateAccessViewModel createAccess();
        public bool createAccess(CreateAccessViewModel model);
        public AccountAccessViewModel accountAccess();
    }
}
