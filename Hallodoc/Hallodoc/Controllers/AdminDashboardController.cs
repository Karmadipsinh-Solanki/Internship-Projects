using HalloDoc.DataLayer.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;
using HalloDoc.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
//using HalloDoc.Data;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ClosedXML;
using ClosedXML.Excel;
//using Microsoft.Data.SqlClient;
using Hallodoc.Data;
using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Models.Models;
using HalloDoc.DataLayer.Models;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
//using System.Diagnostics;
//using HalloDoc.Data;

namespace HalloDoc.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<AdminDashboardController> _logger;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AdminDashboard(string? status)
        {
            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3);
            var count_conclude = _context.Requests.Count(r => r.Status == 4);
            var count_toclose = _context.Requests.Count(r => r.Status == 5);
            var count_unpaid = _context.Requests.Count(r => r.Status == 6);

            AdminDashboardTableView adminDashboardViewModel = new AdminDashboardTableView
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                unpaid_count = count_unpaid,
                query_requests = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 1),
                requests = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 1).ToList(),
                regions = _context.Regions.ToList(),
                status = "New",
            };

            return View(adminDashboardViewModel);
        }

        public IActionResult New()
        {
            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3);
            var count_conclude = _context.Requests.Count(r => r.Status == 4);
            var count_toclose = _context.Requests.Count(r => r.Status == 5);
            var count_unpaid = _context.Requests.Count(r => r.Status == 6);

            IQueryable<Request> req = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 1);
            List<Request> list = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 1).ToList();
            List<Region> region = _context.Regions.ToList();

            AdminDashboardTableView adminDashboardViewModel = new AdminDashboardTableView
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                query_requests = req,
                requests = list,
                regions = region,
                status = "New",
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardViewModel);
        }

        public IActionResult Pending()
        {
            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3);
            var count_conclude = _context.Requests.Count(r => r.Status == 4);
            var count_toclose = _context.Requests.Count(r => r.Status == 5);
            var count_unpaid = _context.Requests.Count(r => r.Status == 6);

            IQueryable<Request> req = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 2);
            List<Request> list = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 2).ToList();
            List<Region> region = _context.Regions.ToList();

            AdminDashboardTableView adminDashboardViewModel = new AdminDashboardTableView
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                query_requests = req,
                requests = list,
                regions = region,
                status = "Pending",
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardViewModel);
        }

        public IActionResult Active()
        {
            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3);
            var count_conclude = _context.Requests.Count(r => r.Status == 4);
            var count_toclose = _context.Requests.Count(r => r.Status == 5);
            var count_unpaid = _context.Requests.Count(r => r.Status == 6);

            IQueryable<Request> req = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 3);
            List<Request> list = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 3).ToList();
            List<Region> region = _context.Regions.ToList();

            AdminDashboardTableView adminDashboardViewModel = new AdminDashboardTableView
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                query_requests = req,
                requests = list,
                regions = region,
                status = "Active",
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardViewModel);
        }

        public IActionResult Conclude()
        {
            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3);
            var count_conclude = _context.Requests.Count(r => r.Status == 4);
            var count_toclose = _context.Requests.Count(r => r.Status == 5);
            var count_unpaid = _context.Requests.Count(r => r.Status == 6);

            IQueryable<Request> req = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 4);
            List<Request> list = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 4).ToList();
            List<Region> region = _context.Regions.ToList();

            AdminDashboardTableView adminDashboardViewModel = new AdminDashboardTableView
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                query_requests = req,
                requests = list,
                regions = region,
                status = "Conclude",
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardViewModel);
        }

        public IActionResult Toclose()
        {
            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3);
            var count_conclude = _context.Requests.Count(r => r.Status == 4);
            var count_toclose = _context.Requests.Count(r => r.Status == 5);
            var count_unpaid = _context.Requests.Count(r => r.Status == 6);

            IQueryable<Request> req = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 5);
            List<Request> list = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 5).ToList();
            List<Region> region = _context.Regions.ToList();

            AdminDashboardTableView adminDashboardViewModel = new AdminDashboardTableView
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                query_requests = req,
                requests = list,
                regions = region,
                status = "ToClose",
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardViewModel);
        }

        public IActionResult Unpaid()
        {
            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3);
            var count_conclude = _context.Requests.Count(r => r.Status == 4);
            var count_toclose = _context.Requests.Count(r => r.Status == 5);
            var count_unpaid = _context.Requests.Count(r => r.Status == 6);

            IQueryable<Request> req = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 6);
            List<Request> list = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 6).ToList();
            List<Region> region = _context.Regions.ToList();

            AdminDashboardTableView adminDashboardViewModel = new AdminDashboardTableView
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                query_requests = req,
                requests = list,
                regions = region,
                status = "Unpaid",
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardViewModel);
        }
        //public List<Request> GetTableData()
        //{ 
        //    List<Request> data = new List<Request>();
        //    var user_id = HttpContext.Session.GetInt32("id");
        //    data = _context.Requests.Include(r => r.RequestClient).Where(u => u.UserId == user_id).ToList();
        //    return data;
        //}

        public IActionResult DownloadExcel()
        {
            try
            {
                List<Request> data = new List<Request>();
                data = _context.Requests.Include(r => r.RequestClient).Include(p => p.Physician).ToList();
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Data");

                worksheet.Cell(1, 1).Value = "Name";
                worksheet.Cell(1, 2).Value = "Date Of Birth";
                worksheet.Cell(1, 3).Value = "Requestor";
                worksheet.Cell(1, 4).Value = "Physician Name";
                worksheet.Cell(1, 5).Value = "Date of Service";
                worksheet.Cell(1, 6).Value = "Requested Date";
                worksheet.Cell(1, 7).Value = "Phone Number";
                worksheet.Cell(1, 8).Value = "Address";
                worksheet.Cell(1, 9).Value = "Notes";

                int row = 2;
                foreach (var item in data)
                {
                    var statusClass = "";
                    var dos = "";
                    var notes = "";
                    if (item.RequestTypeId == 1)
                    {
                        statusClass = "business";
                    }
                    else if (item.RequestTypeId == 4)
                    {
                        statusClass = "patient";
                    }
                    else if (item.RequestTypeId == 2)
                    {
                        statusClass = "family";
                    }
                    else
                    {
                        statusClass = "concierge";
                    }
                    foreach (var stat in item.RequestStatusLogs)
                    {
                        if (stat.Status == 2)
                        {
                            dos = stat.CreatedDate.ToString("MMMM dd,yyyy");
                            notes = stat.Notes ?? "";
                        }
                    }
                    worksheet.Cell(row, 1).Value = item.RequestClient.FirstName + " " + item.RequestClient.LastName;
                    worksheet.Cell(row, 2).Value = DateTime.Parse($"{item.RequestClient.IntYear}-{item.RequestClient.StrMonth}-{item.RequestClient.IntDate}").ToString("MMMM dd,yyyy");
                    worksheet.Cell(row, 3).Value = statusClass.Substring(0, 1).ToUpper() + statusClass.Substring(1).ToLower() + " " + item.FirstName + " " + item.LastName;
                    worksheet.Cell(row, 4).Value = ("Dr." + item?.Physician == null ? "" : item?.Physician?.FirstName);
                    worksheet.Cell(row, 5).Value = item.CreatedDate.ToString("MMMM dd,yyyy");
                    worksheet.Cell(row, 6).Value = dos;
                    worksheet.Cell(row, 7).Value = item.RequestClient.PhoneNumber + " (Patient) " + " " + (item.RequestTypeId != 4 ? item.PhoneNumber + "(" + statusClass.Substring(0, 1).ToUpper() + statusClass.Substring(1).ToLower() + ")" : "");
                    worksheet.Cell(row, 8).Value = (item.RequestClient.Address != null ? item.RequestClient.Address + ", " + item.RequestClient.Street + ", " + item.RequestClient.City + ", " + item.RequestClient.State + ", " + item.RequestClient.ZipCode : item.RequestClient.Street + ", " + item.RequestClient.City + ", " + item.RequestClient.State + ", " + item.RequestClient.ZipCode);
                    worksheet.Cell(row, 9).Value = notes;
                    row++;
                }
                worksheet.Columns().AdjustToContents();

                var memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "data.xlsx");
            }
            catch (Exception ex)
            {
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
        public IActionResult patientdataintable()
        {
            //int id = int.Parse(buttonValue);
            //var data = _context.DevicesList.FirstOrDefault(d => d.Device == id);
            //return View(data);


            var count_patient = _context.Requests.Count(r => r.RequestTypeId == 1);
            var count_family = _context.Requests.Count(r => r.RequestTypeId == 2);
            var count_concierge = _context.Requests.Count(r => r.RequestTypeId == 3);
            var count_business = _context.Requests.Count(r => r.RequestTypeId == 4);

            IQueryable<Request> req = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs);
            List<Request> list = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.RequestTypeId == 1).ToList();
            List<Region> region = _context.Regions.ToList();

            AdminDashboardReqWiseTableView adminDashboardReqWiseViewModel = new AdminDashboardReqWiseTableView
            {
                patient_count = count_patient,
                family_count = count_family,
                concierge_count = count_concierge,
                business_count = count_business,
                query_requests = req,
                requests = list,
                regions = region,
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardReqWiseViewModel);
        }
        public IActionResult familydataintable()
        {
            var count_family = _context.Requests.Count(r => r.RequestTypeId == 2);

            IQueryable<Request> req = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs);
            List<Request> list = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.RequestTypeId == 2).ToList();
            List<Region> region = _context.Regions.ToList();

            AdminDashboardReqWiseTableView adminDashboardReqWiseViewModel = new AdminDashboardReqWiseTableView
            {
                family_count = count_family,
                query_requests = req,
                requests = list,
                regions = region,
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardReqWiseViewModel);
        }
        public IActionResult conciergedataintable()
        {
            var count_concierge = _context.Requests.Count(r => r.RequestTypeId == 3);

            IQueryable<Request> req = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs);
            List<Request> list = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.RequestTypeId == 3).ToList();
            List<Region> region = _context.Regions.ToList();

            AdminDashboardReqWiseTableView adminDashboardReqWiseViewModel = new AdminDashboardReqWiseTableView
            {
                concierge_count = count_concierge,
                query_requests = req,
                requests = list,
                regions = region,
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardReqWiseViewModel);
        }
        public IActionResult businessdataintable()
        {
            var count_business = _context.Requests.Count(r => r.RequestTypeId == 4);

            IQueryable<Request> req = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs);
            List<Request> list = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.RequestTypeId == 4).ToList();
            List<Region> region = _context.Regions.ToList();

            AdminDashboardReqWiseTableView adminDashboardReqWiseViewModel = new AdminDashboardReqWiseTableView
            {
                business_count = count_business,
                query_requests = req,
                requests = list,
                regions = region,
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardReqWiseViewModel);
        }


        public IActionResult ViewCase(int id)
        {
            ViewCaseModel viewCaseModel = new ViewCaseModel();
            var data = _context.Requests.Include(u => u.RequestClient).FirstOrDefault(u => u.RequestId == id);
            var status = "Reserved";
            if (data.Status == 1)
            {
                status = "New";
            }
            viewCaseModel.Status = status;
            viewCaseModel.Email = data?.RequestClient?.Email;
            viewCaseModel.PhoneNumber = data.RequestClient.PhoneNumber;
            viewCaseModel.Room = data.RequestClient.Street;
            viewCaseModel.BusinessAddress = data.RequestClient.Address + " " + data.RequestClient.City;
            viewCaseModel.ConfirmationNo = data.ConfirmationNumber;
            viewCaseModel.PatientNotes = data.RequestClient.Notes;
            viewCaseModel.FirstName = data.RequestClient.FirstName;
            viewCaseModel.LastName = data.RequestClient.LastName;
            viewCaseModel.RequestId = data.RequestId;
            viewCaseModel.Region = data.RequestClient.State;
            int requestId = data.RequestTypeId;
            var requestor = "";
            if (requestId == 1)
            {
                requestor = "Patient";
            }
            else if (requestId == 2)
            {
                requestor = "Family/Friend";
            }
            else if (requestId == 3)
            {
                requestor = "Conceirge";
            }
            else
            {
                requestor = "Business";
            }
            viewCaseModel.Requestor = requestor;
            //var monthName = data.RequestClient.StrMonth;
            
            // Assuming data.RequestClient.StrMonth is a string containing the full month name

            try
            {
                var month1 = data.RequestClient.StrMonth;
                string monthName;
                switch (month1)
                {
                    case "1":
                        monthName = "January";
                        break;
                    case "2":
                        monthName = "February";
                        break;
                    case "3":
                        monthName = "March";
                        break;
                    case "4":
                        monthName = "April";
                        break;
                    case "5":
                        monthName = "May";
                        break;
                    case "6":
                        monthName = "June";
                        break;
                    case "7":
                        monthName = "July";
                        break;
                    case "8":
                        monthName = "August";
                        break;
                    case "9":
                        monthName = "September";
                        break;
                    case "10":
                        monthName = "October";
                        break;
                    case "11":
                        monthName = "November";
                        break;
                    case "12":
                        monthName = "December";
                        break;
                    default:
                        monthName = "Invalid month";
                        break;
                }
                int month = DateTime.ParseExact(monthName, "MMMM", new CultureInfo("en-US")).Month;
                viewCaseModel.DOB = new DateTime((int)data.RequestClient.IntYear, month, (int)data.RequestClient.IntDate);
            }
            catch (FormatException ex)
            {
                // Handle parsing error gracefully, e.g., log the exception or provide a user-friendly message
            }
            return View(viewCaseModel);
        }


        //public IActionResult ViewCase()
        //{
        //    var user = HttpContext.Session.GetInt32("int id");
        //    var userDetail = _context.Users.FirstOrDefault(u => u.UserId == user);
        //    //var userDetail = _profile.UserIdFromUserInProfile((int)user);
        //    ViewCaseModel viewCaseModel = new ViewCaseModel()
        //    {
        //        FirstName = userDetail.FirstName,
        //        LastName = userDetail.LastName,
        //        DOB = DateTime.Parse(userDetail.IntYear.ToString() + '-' + userDetail.StrMonth + '-' + userDetail.IntDate.ToString()),
        //        //meViewModel.DOB = new DateTime(userDetail.IntYear, DateTime.ParseExact(userDetail.StrMonth, "MMMM", CultureInfo.CurrentCulture).Month, userDetail.IntDate);
        //        PhoneNumber = userDetail.Mobile,
        //        Email = userDetail.Email,
        //        Region = userDetail.Region,
        //        Street = userDetail.Street,
        //        State = userDetail.State,
        //        ZipCode = userDetail.ZipCode,
        //        //partialViewModel = new partialViewModel() { patient_name = string.Concat(userDetail.FirstName + ' ' + userDetail) },

        //    };


        //    return View(ViewCase);
        //}
        
        [HttpPost]
        public IActionResult ViewCase(ViewCaseModel userProfile)
        {
            int requestId = userProfile.RequestId;
            if (requestId != null)
            {
                var rid = _context.Requests.Where(u => u.RequestId == requestId).FirstOrDefault();
                var userToUpdate = _context.RequestClients.Where(u => u.RequestClientId == rid.RequestClientId).FirstOrDefault();
                if (userToUpdate != null)
                {
                    userToUpdate.FirstName = userProfile.FirstName;
                    userToUpdate.LastName = userProfile.LastName;
                    userToUpdate.PhoneNumber = userProfile.PhoneNumber;
                    userToUpdate.Email = userProfile.Email;
                    userToUpdate.IntDate = userProfile.DOB.Day;
                    userToUpdate.IntYear = userProfile.DOB.Year;
                    userToUpdate.StrMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(userProfile.DOB.Month);
                    _context.RequestClients.Update(userToUpdate);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("ViewCase", new { requestId = requestId });
        }
        

        public IActionResult ViewNotes(int id)
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
    }
}
