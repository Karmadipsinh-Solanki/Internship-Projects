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

namespace HalloDoc.Controllers
{
    [CustomAuthorize("Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminDashboardController> _logger;
        private readonly IAdmin _admin;
        //private readonly IPatient _patient;
        private readonly IJwtService _jwtService;

        public AdminDashboardController(ApplicationDbContext context, IAdmin admin, IJwtService jwtService)
        {
            _context = context;
            _admin = admin;
            //_patient = patient;
            _jwtService = jwtService;
        }

        public IActionResult adminDashboard(string? status)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("New", null, -1, null);
            return View(adminDashboardTableView);
        }

        public IActionResult New(string? search, int? region, string? requestor)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("New", search, region, requestor);
            return PartialView("AdminDashboardTablePartialView", adminDashboardTableView);
        }

        public IActionResult Pending(string? search, int? region, string? requestor)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("Pending", search, region, requestor);
            return PartialView("AdminDashboardTablePartialView", adminDashboardTableView);
        }

        public IActionResult Active(string? search, int? region, string? requestor)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("Active", search, region, requestor);
            return PartialView("AdminDashboardTablePartialView", adminDashboardTableView);
        }

        public IActionResult Conclude(string? search, int? region, string? requestor)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("Conclude", search, region, requestor);
            return PartialView("AdminDashboardTablePartialView", adminDashboardTableView);
        }

        public IActionResult ToClose(string? search, int? region, string? requestor)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("ToClose", search, region, requestor);
            return PartialView("AdminDashboardTablePartialView", adminDashboardTableView);
        }

        public IActionResult Unpaid(string? search, int? region, string? requestor)
        {
            AdminDashboardTableView adminDashboardTableView = _admin.adminDashboard("Unpaid", search, region, requestor);
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
        public IActionResult VerifyState(CreateRequestViewModel model)
        {
            bool check = _admin.verifyState(model);
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
            return RedirectToAction("ViewCase", new { requestId = requestId });
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
            List<Hallodoc.Region> regions = _admin.fetchRegions();
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
        [HttpPost]
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
            return RedirectToAction("CloseCase", new { requestId = model.RequestId });
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
            List<Hallodoc.Region> regions = _admin.fetchAdminRegions();
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
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
