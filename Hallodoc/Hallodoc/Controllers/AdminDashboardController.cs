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
//using HalloDoc.DataLayer.Data;

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
        [ValidateAntiForgeryToken]
        public IActionResult AdminDashboard(AdminDashboardTableView model)
        {
            AdminDashboardTableView viewmodel = _admin.adminDashboard(model.status, model.search, model.RegionId, model.requestor, (int)model.CurrentPage, (int)model.PageSize);
            MemoryStream memoryStream = _admin.Export(viewmodel);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Data-{model.status}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> SendLink(AdminDashboardTableView model)
        {
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            int isVerified = 0;
            if (region == null)
            {
                isVerified = 2;
                return Json(new { isVerified = isVerified });
            }
            bool check = _admin.verifyState(region);
            if (check)
            {
                isVerified = 1;
            }
            else
            {
                isVerified = 3;
            }
            return Json(new { isVerified = isVerified });
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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

            AdminProfileViewModel adminProfileViewModel = _admin.adminProfile();
            return View(adminProfileViewModel);
        }
        public IActionResult AccountAccess()
        {
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

            AccountAccessViewModel accountAccessViewModel = _admin.accountAccess();
            return View(accountAccessViewModel);
        }
        public IActionResult CreateAccess()
        {
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

            return View();
        }
        public IActionResult SearchRecords()
        {
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

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
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

            BlockHistoryViewModel blockHistoryViewModel = _admin.blockHistory(null, null, null, null);
            return View(blockHistoryViewModel);
        }
        public IActionResult PatientBlockHistory(string? firstname, DateTime? date, string? email, string? phonenumber, int page = 1, int pageSize = 10)
        {
            BlockHistoryViewModel blockHistoryViewModel = _admin.blockHistory(firstname, date, email, phonenumber, page, pageSize);
            return PartialView("BlockHistoryTable", blockHistoryViewModel);
        }
        /// <summary>
        /// emailLogTable page controller
        /// </summary>
        /// <returns></returns>
        public IActionResult EmailLog()
        {
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

            EmailLogViewModel emailLogViewModel = _admin.emailLog(null, null, null, null, null);
            return View(emailLogViewModel);
        }
        public IActionResult EmailLogFilter(string? receiverName, DateTime? date, DateTime? date2, string? email, string? role, int page = 1, int pageSize = 10)
        {
            EmailLogViewModel emailLogViewModel = _admin.emailLog(receiverName, date, date2, email, role, page, pageSize);
            return PartialView("EmailLogTable", emailLogViewModel);
        }

        /// <summary>
        /// This is vendors/partners controllers that contains edit and delete business
        /// </summary>
        /// <returns></returns>
        /// 
        //public IActionResult AddBusiness(int id = -1)
        //{
        //    AddBusinessViewModel addBusinessViewModel = _admin.addBusiness(id);
        //    return View(addBusinessViewModel);
        //}
        //public IActionResult EditBusiness(AddBusinessViewModel model)
        //{
        //    bool check = _admin.editBusiness(model);
        //    if (check)
        //    {
        //        TempData["success"] = "Business details updated successfully!";
        //    }
        //    else
        //    {
        //        TempData["error"] = "Error,business details not updated!";
        //    }
        //    return RedirectToAction("Vendors");
        //}
        //[HttpPost]
        //public IActionResult AddBusiness(AddBusinessViewModel model)
        //{
        //    bool check = _admin.addBusiness(model);
        //    if (check)
        //    {
        //        TempData["success"] = "Business added successfully!";
        //    }
        //    else
        //    {
        //        TempData["error"] = "Error,business not added!";
        //    }
        //    return RedirectToAction("Vendors");
        //}



        //public IActionResult AddBusiness()
        //{
        //    AddBusinessViewModel addBusinessViewModel = _admin.addBusiness();
        //    return View();
        //}
        public IActionResult DeleteVendor(int id)
        {
            bool check = _admin.deleteVendor(id);
            if (check)
            {
                TempData["success"] = "Vendor deleted successfully!";
            }
            else
            {
                TempData["error"] = "Error,Vendor not deleted!";
            }
            return RedirectToAction("Vendors");
        }
        public IActionResult FilterVendorInformation(string? vendorName, int? professionType, int page = 1, int pageSize = 10)
        {
            VendorViewModel vendorViewModel = _admin.vendorInformation(vendorName, professionType, page, pageSize);
            return PartialView("VendorTable", vendorViewModel);
        }
        public IActionResult Vendors()
        {
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

            VendorViewModel vendorViewModel = _admin.vendorInformation(null, null);
            return View(vendorViewModel);
        }
        public IActionResult Scheduling()
        {
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

            ProviderShift providerShift = _admin.scheduling();
            return View(providerShift);
        }
        public IActionResult CreateShift(ProviderShift model)
        {
            bool check = _admin.createShift(model);
            if (check)
            {
                TempData["success"] = "Shift created successfully!";
            }
            else
            {
                TempData["error"] = "Error,shift not created!";
            }
            return RedirectToAction("Scheduling");
        }
        public IActionResult ViewShift(int id)
        {
            var shiftDetail = _admin.viewShift(id);
            return Json(new { response = shiftDetail });
        }
        public IActionResult EditShift(ProviderShift model)
        {
            bool check = _admin.editShift(model);
            if (check)
            {
                TempData["success"] = "Shift updated successfully!";
            }
            else
            {
                TempData["error"] = "Error,shift not updated!";
            }
            return RedirectToAction("Scheduling");
        }
        public IActionResult DeleteShift(int id)
        {
            bool check = _admin.deleteShift(id);
            if (check)
            {
                TempData["success"] = "Shift deleted successfully!";
            }
            else
            {
                TempData["error"] = "Error,shift not deleted!";
            }
            return Json(new { response = "success" });
        }
        public IActionResult GetProviderDetailsForSchedule(int RegionId)
        {
            List<ProviderInformationViewModel> model = _admin.GetProviderInformation(RegionId);

            List<ProviderDTO> list = model.Select(p => new ProviderDTO
            {
                Id = p.PhysicianId,
                title = p.ProviderName ?? "",
                imageUrl = "/uploads/physician/" + p.PhysicianId + "/photo.jpg",
            }).ToList();
            return Json(list);
        }
        public IActionResult GetScheduleData(int RegionId)
        {
            string[] color = { "#edacd2", "#a5cfa6" };
            List<ShiftDetail> shiftDetails = _admin.GetScheduleData(RegionId);

            List<ShiftDTO> list = shiftDetails.Select(s => new ShiftDTO
            {
                resourceId = s.Shift.PhysicianId,
                Id = s.ShiftDetailId,
                title = s.StartTime + " - " + s.EndTime + " " + _admin.GetPhyFromId(s.Shift.PhysicianId, s.ShiftId),
                start = s.ShiftDate.ToString("yyyy-MM-dd") + s.StartTime.ToString("THH:mm:ss"),
                end = s.ShiftDate.ToString("yyyy-MM-dd") + s.EndTime.ToString("THH:mm:ss"),
                region = (int)s.RegionId,
                color = color[s.Status]
            }).ToList();
            return Json(list);
        }
        public IActionResult ViewCurrentMonth()
        {
            var currentMonth = DateTime.Now.Month.ToString();
            TempData["currentMonth"] = currentMonth;
            return RedirectToAction("Scheduling");
        }

        public IActionResult FilterShiftDetail(int? region, int page = 1, int pageSize = 10)
        {
            ShiftReviewViewModel shiftReviewViewModel = _admin.filterShiftDetail(region, page, pageSize);
            return PartialView("ShiftReviewTable", shiftReviewViewModel);
        }
        public IActionResult ShiftForReview()
        {
            ShiftReviewViewModel shiftReviewViewModel = _admin.filterShiftDetail(null);
            return View(shiftReviewViewModel);
        }
        public IActionResult ReturnShift(int id)
        {
            bool check = _admin.returnShift(id);
            if (check)
            {
                TempData["success"] = "Shift returned successfully!";
            }
            else
            {
                TempData["error"] = "Error,shift not returned!";
            }
            return Json(new { response = "success" });
        }
        //create provider
        public IActionResult ContactProvider(ProviderViewModel model)
        {
            bool check = _admin.contactProvider(model);
            if (check)
            {
                TempData["success"] = "Message sent to provider successfully!";
            }
            else
            {
                TempData["error"] = "Error,message not sent!";
            }
            return RedirectToAction("ProviderInformation");
        }
        [HttpPost]
        public IActionResult EditPhysicianProfile(CreateProviderViewModel model)
        {
            bool check = _admin.editPhysicianProfile(model);
            if (check)
            {
                TempData["success"] = "Provider editted successfully!";
            }
            else
            {
                TempData["error"] = "Error,provider not editted!";
            }
            return RedirectToAction("EditPhysicianAccount", new { id = model.PhysicianId });
        }
        [HttpPost]
        public IActionResult EditPhysicianOnboarding(CreateProviderViewModel model)
        {
            bool check = _admin.editPhysicianOnboarding(model);
            if (check)
            {
                TempData["success"] = "Provider editted successfully!";
            }
            else
            {
                TempData["error"] = "Error,provider not editted!";
            }
            return RedirectToAction("EditPhysicianAccount", new { id = model.PhysicianId });
        }
        [HttpPost]
        public IActionResult EditPhysicianInformation(CreateProviderViewModel model)
        {
            bool check = _admin.editPhysicianInformation(model);
            if (check)
            {
                TempData["success"] = "Provider editted successfully!";
            }
            else
            {
                TempData["error"] = "Error,provider not editted!";
            }
            return RedirectToAction("EditPhysicianAccount", new { id = model.PhysicianId });
        }
        [HttpPost]
        public IActionResult EditPhysicianMailingInformation(CreateProviderViewModel model)
        {
            bool check = _admin.editPhysicianMailingInformation(model);
            if (check)
            {
                TempData["success"] = "Provider editted successfully!";
            }
            else
            {
                TempData["error"] = "Error,provider not editted!";
            }
            return RedirectToAction("EditPhysicianAccount", new { id = model.PhysicianId });
        }
        public IActionResult DeletePhysicianAccount(int id)
        {
            bool check = _admin.deletePhysicianAccount(id);
            if (check)
            {
                TempData["success"] = "Provider deleted successfully!";
            }
            else
            {
                TempData["error"] = "Error,provider not deleted!";
            }
            return RedirectToAction("ProviderInformation");
        }

        public IActionResult EditPhysicianPassword(string password, int id)
        {
            bool check = _admin.editPhysicianPassword(password, id);
            if (check)
            {
                TempData["success"] = "Password updated successfully!";
            }
            else
            {
                TempData["error"] = "Error,password not updated!";
            }
            return Json(new { response = "success" });
        }
        [HttpPost]
        public IActionResult EditPhysicianAccountInformation(CreateProviderViewModel model)
        {
            bool check = _admin.editPhysicianAccountInformation(model);
            if (check)
            {
                TempData["success"] = "Provider editted successfully!";
            }
            else
            {
                TempData["error"] = "Error,provider not editted!";
            }
            return RedirectToAction("EditPhysicianAccount", new { id = model.PhysicianId });
        }
        //[HttpPost]
        //public IActionResult EditPhysician(CreateProviderViewModel model)
        //{
        //    bool check = _admin.editPhysicianAccount(model);
        //    if (check)
        //    {
        //        TempData["success"] = "Provider editted successfully!";
        //    }
        //    else
        //    {
        //        TempData["error"] = "Error,provider not editted!";
        //    }
        //    return RedirectToAction("ProviderInformation");
        //}
        public IActionResult EditPhyAccount(int id)
        {
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

            CreateProviderViewModel createProviderViewModel = _admin.editPhysicianAccount(id);
            return View(createProviderViewModel);
        }
        [HttpPost]
        public IActionResult CreatePhysician(CreateProviderViewModel model)
        {
            bool check = _admin.createPhysician(model);
            if (check)
            {
                TempData["success"] = "Provider created successfully!";
            }
            else
            {
                TempData["error"] = "Error,provider not created!";
            }
            return RedirectToAction("ProviderInformation");
        }
        public IActionResult CreatePhysician()
        {
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            List<string> roleMenu = _admin.GetListOfRoleMenu(cookieModel.AccessRoleId);
            ViewBag.Menu = roleMenu;

            CreateProviderViewModel createProviderViewModel = _admin.createPhysician();
            return View(createProviderViewModel);
        }
        //public IActionResult ProviderLocation()
        //{
        //    ProviderLocationViewModel providerLocationViewModel = _admin.providerLocation();
        //    return View(providerLocationViewModel);
        //}
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
