using HalloDoc.DataLayer.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;
using HalloDoc.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ClosedXML;
using ClosedXML.Excel;
using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Models.Models;
using HalloDoc.DataLayer.Models;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;
using HalloDoc.ViewModels;
using System.Net.Mail;
using System.Net;
using System.Collections;
using HalloDoc.Repository.Interface;
using HalloDoc.Repository.Auth;
using System.Drawing;
using Hallodoc;
using HalloDoc.LogicLayer.Interface;
using HalloDoc.LogicLayer.Repository;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DocumentFormat.OpenXml.Office2010.Excel;
using HalloDoc.DataLayer.Data;

namespace HalloDoc.Controllers
{
    [CustomAuthorize("Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminDashboardController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAdmin _admin;
        //private readonly IPatient _patient;
        private readonly IJwtService _jwtService;

        public AdminDashboardController(ApplicationDbContext context, IAdmin admin, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _admin = admin;
            //_patient = patient;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult adminDashboard(string? status)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("New", null, -1, null);
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

            return View(adminDashboardTableView);
        }

        public IActionResult New(string? search, int? region, string? requestor,int page=1,int pageSize=10)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("New", search, region, requestor,page,pageSize);
            return PartialView("AdminDashboardTablePartialView", adminDashboardTableView);
        }

        public IActionResult Pending(string? search, int? region, string? requestor, int page = 1, int pageSize = 10)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("Pending", search, region, requestor,page,pageSize);
            return PartialView("AdminDashboardTablePartialView", adminDashboardTableView);
        }

        public IActionResult Active(string? search, int? region, string? requestor, int page = 1, int pageSize = 10)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("Active", search, region, requestor, page, pageSize);
            return PartialView("AdminDashboardTablePartialView", adminDashboardTableView);
        }

        public IActionResult Conclude(string? search, int? region, string? requestor, int page = 1, int pageSize = 10)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("Conclude", search, region, requestor, page, pageSize);
            return PartialView("AdminDashboardTablePartialView", adminDashboardTableView);
        }

        public IActionResult ToClose(string? search, int? region, string? requestor, int page = 1, int pageSize = 10)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("ToClose", search, region, requestor, page, pageSize);
            return PartialView("AdminDashboardTablePartialView", adminDashboardTableView);
        }

        public IActionResult Unpaid(string? search, int? region, string? requestor, int page = 1, int pageSize = 10)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("Unpaid", search, region, requestor, page, pageSize);
            return PartialView("AdminDashboardTablePartialView", adminDashboardTableView);
        }
        //public List<Request> GetTableData()
        //{ 
        //    List<Request> data = new List<Request>();
        //    var user_id = HttpContext.Session.GetInt32("id");
        //    data = _context.Requests.Include(r => r.RequestClient).Where(u => u.UserId == user_id).ToList();
        //    return data;
        //}

        
        [HttpPost]
        public async Task<IActionResult> SendLink(AdminDashboardTableView model)
        {
            bool check = _admin.sendLink(model);
            if (check)
            {
                TempData["success"] = "Link sent successfully!";
            }
            else
            {
                TempData["error"] = "Link not sent!";
            }
            return RedirectToAction("AdminDashboard");
        }
        public IActionResult CreateRequest()
        {
            CreateRequestViewModel createRequestViewModel = _admin.createRequest();
            return View(createRequestViewModel);
        }
        [HttpPost]
        public IActionResult CreateRequest(CreateRequestViewModel model)
        {
            int check = _admin.createRequest(model);
            if (check == 0)
            {
                ModelState.AddModelError("State", "Currently we are not serving in this region");
                return View(model);
            }
            else if (check == 1)
            {
                ModelState.AddModelError("Email", "This patient is blocked.");
                return View(model);
            }
            else
            {
                TempData["success"] = "Request created successfully!";
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult VerifyState(string? region)
        {
            bool check = _admin.verifyState(region);
            if (check)
            {
                TempData["success"] = "We are serving in this region!";
            }
            else
            {
                TempData["error"] = "Currently, We are not serving in this region!";
            }
            return RedirectToAction("CreateRequest");
        }


        public IActionResult DownloadExcel()
        {
            try
            {
                MemoryStream memoryStream = _admin.downloadExcel();
                TempData["success"] = "Excel downloaded successfully!";
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "data.xlsx");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error in downloading excel!";
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
        //public IActionResult ViewCase()
        //{
        //    return View();
        //}

        //for showing table according to all,patient,concierge,business
        //public IActionResult patientdataintable()
        //{
        //    //int id = int.Parse(buttonValue);
        //    //var data = _context.DevicesList.FirstOrDefault(d => d.Device == id);
        //    //return View(data);


        //    var count_patient = _context.Requests.Count(r => r.RequestTypeId == 1);
        //    var count_family = _context.Requests.Count(r => r.RequestTypeId == 2);
        //    var count_concierge = _context.Requests.Count(r => r.RequestTypeId == 3);
        //    var count_business = _context.Requests.Count(r => r.RequestTypeId == 4);

        //    IQueryable<Request> req = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs);
        //    List<Region> regions = _context.Regions.ToList();

        //    AdminDashboardReqWiseTableView adminDashboardReqWiseViewModel = new AdminDashboardReqWiseTableView
        //    {
        //        patient_count = count_patient,
        //        family_count = count_family,
        //        concierge_count = count_concierge,
        //        business_count = count_business,
        //        query_requests = req,
        //        requests = list,
        //        regions = regions,
        //    };
        //    return PartialView("AdminDashboardTablePartialView", adminDashboardReqWiseViewModel);
        //}


        public IActionResult ViewCase(int id)
        {
            ViewCaseModel viewCaseModel = _admin.viewCase(id);
            return View(viewCaseModel);
        }

        [HttpPost]
        public IActionResult ViewCase(ViewCaseModel model)
        {
            int requestId = model.RequestId;
            bool check = _admin.viewCase(model);
            if (check)
            {
                TempData["success"] = "Case editted successfully";
            }
            else
            {
                TempData["error"] = "Request does not exists. Case not updated!";
            }
            return RedirectToAction("ViewCase", new { id = requestId });
        }
        [HttpPost]
        public IActionResult viewCaseAssignModal(ViewCaseModel model)
        {
            int requestId = model.RequestId;
            bool check = _admin.viewCaseAssignModal(model);
            if (check)
            {
                TempData["success"] = "Case assigned successfully";
            }
            else
            {
                TempData["error"] = "Case not assigned!";
            }
            return RedirectToAction("ViewCase", new { id = requestId });
        }


        public IActionResult ViewNotes(int id)
        {
            ViewNotesViewModel viewNotesViewModel = _admin.viewNotes(id);
            return View(viewNotesViewModel);
        }

        //bool IAdmin.updateAdminNotes(ViewNotesViewModel viewNotesViewModel)
        [HttpPost]
        public IActionResult ViewNotes(ViewNotesViewModel viewNotesViewModel)
        {
            int requestId = viewNotesViewModel.RequestId;
            var check = _admin.viewNotes(viewNotesViewModel);
            if (check)
            {
                TempData["success"] = "Admin Notes Updated!";
            }
            return RedirectToAction("ViewNotes", new { id = viewNotesViewModel.RequestId });
        }
        
        public IActionResult fetchBusiness(int id)
        {
            List<HealthProfessional> business = _admin.fetchBusiness(id);
            return Json(business);
        }public IActionResult fetchBusinessDetail(int id)
        {
            HealthProfessional business = _admin.fetchBusinessDetail(id);
            return Json(business);
        }
        public IActionResult FetchTags()
        {
            // Replace with your actual logic to fetch regions
            //var regions = _context.CaseTags.Select(r => new { Id = r.CaseTagId, Name = r.Name }).ToList();
            List<CaseTag> regions = _admin.fetchTags();
            return Json(regions);
        }
        public IActionResult FetchRegions()
        {
            //var regions = _context.Regions.Select(r => new { Id = r.RegionId, Name = r.Name }).ToList();
            List<HalloDoc.DataLayer.Models.Region> regions = _admin.fetchRegions();
            return Json(regions);
        }

        public IActionResult FetchPhysicians(int id)
        {
            //var physicians = _context.Physicians.Where(r => r.RegionId == id).ToList();
            List<Physician> physicians = _admin.fetchPhysicians(id);
            return Json(physicians);
        }

        [HttpPost]
        public IActionResult AssignCase(AdminDashboardTableView model)
        {
            bool check = _admin.assignCase(model);
            if (check)
            {
                TempData["success"] = "Case assigned successfully!";
            }
            else
            {
                TempData["error"] = "Case is not assigned!";
            }
            return RedirectToAction("AdminDashboard");
        }
        [HttpPost]
        public IActionResult TransferCase(AdminDashboardTableView model)
        {
            bool check = _admin.transferCase(model);
            if (check)
            {
                TempData["success"] = "Case transferred successfully!";
            }
            else
            {
                TempData["error"] = "Case is not transferred!";
            }
            return RedirectToAction("AdminDashboard");
        }

        [HttpPost]
        public IActionResult CancelCase(AdminDashboardTableView model)
        {
            bool check = _admin.cancelCase(model);
            if (check)
            {
                TempData["success"] = "Case cancelled successfully!";
            }
            else
            {
                TempData["error"] = "Case not cancelled!";
            }
            return RedirectToAction("AdminDashboard");
        }
        [HttpPost]
        public IActionResult ClearCase(AdminDashboardTableView model)
        {
            bool check = _admin.clearCase(model);
            if (check)
            {
                TempData["success"] = "Case cleared successfully!";
            }
            else
            {
                TempData["error"] = "Case not cleared!";
            }
            return RedirectToAction("AdminDashboard");
        }
        [HttpPost]
        public IActionResult BlockCase(AdminDashboardTableView model)
        {
            bool check = _admin.blockCase(model);
            if (check)
            {
                TempData["success"] = "Case blocked successfully!";
            }
            else
            {
                TempData["error"] = "Case is not blocked!";
            }
            return RedirectToAction("AdminDashboard");
        }

        //view upload

        //public async Task<IActionResult> deleteSingleFile(int id) 
        //{
        //    RequestWiseFile requestWiseFile = _context.RequestWiseFiles.FirstOrDefault(r => r.RequestWiseFileId == id);
        //    requestWiseFile.IsDeleted = new BitArray(new[] { true });
        //        _context.RequestWiseFiles.Update(requestWiseFile);
        //        _context.SaveChanges();
        //    return requestWiseFile.RequestId;
        //}
        public IActionResult viewUpload(int id)
        {
            ViewUploadViewModel viewUploadViewModel = _admin.viewUpload(id);
            return View(viewUploadViewModel);
        }
        [HttpPost]
        public IActionResult viewUpload(ViewUploadViewModel model, int id)
        {
            bool check = _admin.viewUpload(model);
            if (check)
            {
                TempData["success"] = "Document Uploaded successfully!";
            }
            else
            {
                TempData["error"] = "Document is not Uploaded!";
            }
            return RedirectToAction("viewUpload");
        }
        public IActionResult MailDocument(string requestFilesIdString, int requestId)
        {
            int id = requestId;
            List<int> requestFilesId = requestFilesIdString.Split(',').Select(int.Parse).ToList();
            bool check = _admin.mailDocument(requestFilesId, requestId);
            if (check)
            {
                TempData["success"] = "Mail sent successfully!";
            }
            else
            {
                TempData["error"] = "Error,Mail not sent";
            }
            return Json(new { res = "success" });
        }
        public IActionResult DeleteViewUploadFile(string fileids, int id)
        {
            bool check = _admin.deleteViewUploadFile(fileids);
            if (check)
            {
                ViewData["success"] = "File deleted successfully";
            }
            else
            {
                ViewData["error"] = "Error, file not selected";
            }
            return Json(new { response = "success" });
        }
        public IActionResult SendOrder(int id)
        {
            OrderViewModel orderViewModel = _admin.SendOrder(id);
            return View(orderViewModel);
        }
        [HttpPost]
        public IActionResult SendOrder(OrderViewModel model)
        {
            bool check = _admin.SendOrder(model);
            if (check)
            {
                TempData["success"] = "Order sent successfully!";
            }
            else
            {
                TempData["error"] = "Error,Order not sent!";
            }
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult CloseCase(int id)
        {
            ViewUploadViewModel viewUploadViewModel = _admin.closeCase(id);
            //return View(viewUploadViewModel);
            //return RedirectToAction("CloseCase", new { id = viewUploadViewModel.RequestId });
            return View(viewUploadViewModel);
        }
        public IActionResult CloseCaseBtn(int requestId)
        {

            bool check = _admin.closeCaseBtn(requestId);
            if (check)
            {
                TempData["success"] = "Case closed successfully!";
            }
            else
            {
                TempData["error"] = "Error,case not closed!";
            }
            return RedirectToAction("AdminDashboard");
        }
        [HttpPost]
        public IActionResult CloseCaseSaveBtn(ViewUploadViewModel model)
        {
            bool check = _admin.closeCaseSaveBtn(model);
            if (check)
            {
                TempData["success"] = "Patient Information updated successfully!";
            }
            else
            {
                TempData["error"] = "Error,Patient Information not updated!";
            }
            return RedirectToAction("CloseCase", new { id = model.RequestId });
        }
        public IActionResult Encounter(int id)
        {
            EncounterViewModel encounterViewModel = _admin.encounter(id);
            return View(encounterViewModel);
        }
        [HttpPost]
        public IActionResult Encounter(EncounterViewModel model)
        {
            bool check = _admin.encounter(model);
            if (check)
            {
                TempData["success"] = "Form editted successfully";
            }
            else
            {
                TempData["error"] = "Error,form could not be editted";
            }
            return RedirectToAction("Encounter", new { requestId = model.RequestId });
        }
        [HttpPost]
        public IActionResult SendAgreement(AdminDashboardTableView model)
        {
            bool check = _admin.sendAgreement(model);
            if (check)
            {
                TempData["success"] = "Agreement sent successfully!";
            }
            else
            {
                TempData["error"] = "Agreement is not sent!";
            }
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult FetchAdminRegions()
        {
            List<HalloDoc.DataLayer.Models.Region> regions = _admin.fetchAdminRegions();
            return Json(new { response = regions });
        }
        public IActionResult SaveAdministratorDetail(AdminProfileViewModel model)
        {
            bool check = _admin.saveAdministratorDetail(model);
            if (check)
            {
                TempData["success"] = "Administrator information updated successfully!";
            }
            else
            {
                TempData["error"] = "Error,Administrator Information not updated!";
            }
            return RedirectToAction("AdminProfile");
        }
        [HttpPost]
        public IActionResult SaveBillingInformation(AdminProfileViewModel model)
        {
            bool check = _admin.saveBillingInformation(model);
            if (check)
            {
                TempData["success"] = "Mailing & Billing Information updated successfully!";
            }
            else
            {
                TempData["error"] = "Error,Mailing & Billing Information not updated!";
            }
            return RedirectToAction("AdminProfile");
        }
        public IActionResult ResetPassword(AdminProfileViewModel model)
        {
            bool check = _admin.resetPassword(model);
            if (check)
            {
                TempData["success"] = "Password updated successfully!";
            }
            else
            {
                TempData["error"] = "Error,password not updated!";
            }
            return RedirectToAction("AdminProfile");
        }
       

        public IActionResult AdminProfile()
        {
            AdminProfileViewModel adminProfileViewModel = _admin.adminProfile();
            return View(adminProfileViewModel);
        }
        public IActionResult AccountAccess()
        {
            AccountAccessViewModel accountAccessViewModel = _admin.accountAccess();
            return View(accountAccessViewModel);
        }
        public IActionResult CreateAccess()
        {
            CreateAccessViewModel createAccessViewModel = _admin.createAccess();
            return View(createAccessViewModel);
        }
        [HttpPost]
        public IActionResult CreateAccess(CreateAccessViewModel model)
        {
            bool check = _admin.createAccess(model);
            if (check)
            {
                TempData["success"] = "Account Access created successfully!";
            }
            else
            {
                TempData["error"] = "Error,Access is not created!";
            }
            return RedirectToAction("AccountAccess");
        }
        public IActionResult UserAccess()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateAdmin(CreateAdminViewModel createAdminViewModel)
        {
            bool check = _admin.createAdmin(createAdminViewModel);
            if (check)
            {
                TempData["success"] = "Admin account created successfully!";
            }
            else
            {
                TempData["error"] = "Error,Admin account is not created!";
            }
            return RedirectToAction("AdminDashboard");
        }
        public IActionResult CreateAdmin()
        {
            CreateAdminViewModel createAdminViewModel = _admin.createAdmin();
            return View(createAdminViewModel);
        }
        public IActionResult FilterProviderInformation(int? region, int page = 1, int pageSize = 10)
        {
            ProviderViewModel providerViewModel = _admin.providerInfo(region, page, pageSize);
            return PartialView("ProviderTable", providerViewModel);
        }
        public IActionResult ProviderInfo()
        {
            ProviderViewModel providerViewModel = _admin.providerInfo(null);
            return View(providerViewModel);
        }
        [HttpPost]
        public IActionResult UpdateStopNotification(int id, bool stopNotification)
        {
            //bool check = _admin.updateStopNotification(id, stopNotification);
            return RedirectToAction("ProviderInfo");
        }
        public IActionResult EditPhyAccount()
        {
            return View();
        }
        public IActionResult SearchRecords()
        {
            SearchRecordsViewModel searchRecordsViewModel = _admin.searchRecords(null, null, null, null, null, null, null, null);
            return View(searchRecordsViewModel);
        }
        public IActionResult SearchSearchRecords(string? patientName, string? providerName, string? email, string? phonenumber, int? selectedOptionValue, int? selectRequestType, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            SearchRecordsViewModel searchRecordsViewModel = _admin.searchRecords(patientName, providerName, email, phonenumber, selectedOptionValue, selectRequestType, fromDate, toDate, page, pageSize);
            return PartialView("SearchRecordsTable", searchRecordsViewModel);
        }
        public IActionResult PatientHistory()
        {
            PatientHistoryViewModel patientHistoryViewModel = _admin.patientHistory(null, null, null, null);
            return View(patientHistoryViewModel);
        }
        public IActionResult PatientSearchHistory(string? firstname, string? lastname, string? email, string? phonenumber, int page = 1, int pageSize = 10)
        {
            PatientHistoryViewModel patientHistoryViewModel = _admin.patientHistory(firstname, lastname, email, phonenumber, page, pageSize);
            return PartialView("PatientHistoryTable", patientHistoryViewModel);
        }
        public IActionResult UnBlock(int id)
        {
            bool check = _admin.unBlock(id);
            if (check)
            {
                TempData["success"] = "Patient Unblocked successfully!";
            }
            else
            {
                TempData["error"] = "Error,Patient is not unblocked!";
            }
            return RedirectToAction("BlockHistory");
        }
        /// <summary>
        /// for isActive checkbox in block history page
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateIsActive(int id, bool isActive)
        {
            //var blockRequest = _context.BlockRequests.Find(id);

            //if (blockRequest == null)
            //{
            //    return Json(new { success = false });
            //}

            //blockRequest.IsActive = isActive;
            //_context.SaveChanges();

            //return Json(new { success = true });

            bool check = _admin.updateIsActive(id,isActive);
            //if (check)
            //{
            //    TempData["success"] = "Patient Unblocked successfully!";
            //}
            //else
            //{
            //    TempData["error"] = "Error,Patient is not unblocked!";
            //}
            return RedirectToAction("BlockHistory");
        }
        public IActionResult BlockHistory()
        {
            BlockHistoryViewModel blockHistoryViewModel = _admin.blockHistory(null, null, null, null);
            return View(blockHistoryViewModel);
        }
        public IActionResult PatientBlockHistory(string? firstname, DateTime? date, string? email, string? phonenumber, int page = 1, int pageSize = 10)
        {
            BlockHistoryViewModel blockHistoryViewModel = _admin.blockHistory(firstname, date, email, phonenumber, page, pageSize);
            return PartialView("BlockHistoryTable", blockHistoryViewModel);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
