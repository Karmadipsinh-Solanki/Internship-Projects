using Hallodoc.Models.Models;
using HalloDoc.DataLayer.ViewModels.AdminViewModels;
using HalloDoc.Repository.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HalloDoc.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using HalloDoc.ViewModels;
using ClosedXML.Excel;
using Hallodoc;
using HalloDoc.LogicLayer.Interface;
using HalloDoc.DataLayer.ViewModels;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Collections;
using static HalloDoc.DataLayer.ViewModels.AdminViewModels.ProviderStatus;
using static HalloDoc.DataLayer.ViewModels.AdminViewModels.RequestType;
using HalloDoc.DataLayer.Models;
//using HalloDoc.DataLayer.Data;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IdentityModel.Tokens.Jwt;
using HalloDoc.DataLayer.Data;
using Twilio;
using Microsoft.Extensions.Configuration;
using Twilio.Rest.Api.V2010.Account;

namespace HalloDoc.LogicLayer.Repository
{
    public class Admin : IAdmin
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public Admin(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IJwtService jwtService,IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
            _configuration = configuration;
        }
        //AdminDashboardTableView adminDashboard(string status, string? search, string? requestor, int? region, int page = 1, int pageSize = 10)
        //{

        //}

        public AdminDashboardTableView adminDashboard(string status, string? search, int? region, string? requestor, int page = 1, int pageSize = 10)
        {

            Expression<Func<Request, bool>> exp;
            if (status == "New")
            {
                exp = r => r.Status == 1;
            }
            else if (status == "Pending")
            {
                exp = r => r.Status == 2;
            }
            else if (status == "Active")
            {
                exp = r => r.Status == 3 || r.Status == 4;
            }
            else if (status == "Conclude")
            {
                exp = r => r.Status == 5;
            }
            else if (status == "ToClose")
            {
                exp = r => r.Status == 6 || r.Status == 7 || r.Status == 8;
            }
            else
            {
                exp = r => r.Status == 9;
            }

            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _context.Requests.Count(r => r.Status == 5);
            var count_toclose = _context.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _context.Requests.Count(r => r.Status == 9);

            IQueryable<Request> query = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(exp);
            List<Region> regions = _context.Regions.ToList();

            if (search != null)
            {
                query = query.Where(r => r.RequestClient.FirstName.ToLower().Contains(search.ToLower()) || r.RequestClient.LastName.ToLower().Contains(search.ToLower()));
            }

            if (requestor == "Family")
            {
                query = query.Where(r => r.RequestTypeId == 2);
            }

            if (requestor == "Business")
            {
                query = query.Where(r => r.RequestTypeId == 4);
            }

            if (requestor == "Concierge")
            {
                query = query.Where(r => r.RequestTypeId == 3);
            }

            if (requestor == "Patient")
            {
                query = query.Where(r => r.RequestTypeId == 1);
            }
            if (region != null && region != 0)
            {
                query = query.Where(r => r.RequestClient.RegionId == region);

            }

            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            //ViewBag.Menu = roleMenu;

            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 1;


            //var request2 = _httpContextAccessor.HttpContext.Request;
            //var token = request2.Cookies["jwt"];
            //CookieModel cookieModel = _jwtService.getDetails(token);
            //string AdminName = cookieModel.name;
            //AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            //adminNavbarViewModel.AdminName = AdminName;
            //adminNavbarViewModel.Tab = 1;

            AdminDashboardTableView adminDashboardViewModel = new AdminDashboardTableView
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                unpaid_count = count_unpaid,
                query_requests = query,
                requests = query.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                regions = regions,
                status = status,
                adminNavbarViewModel = adminNavbarViewModel,
                TotalItems = query.Count(),
                TotalPages = (int)Math.Ceiling((double)query.Count() / pageSize),
                PageSize = pageSize,
                CurrentPage = page
            };
            return adminDashboardViewModel;
        }
        public List<string> GetListOfRoleMenu(int? roleId)
        {
            List<RoleMenu> roleMenus = _context.RoleMenus.Where(u => u.RoleId == roleId).ToList();
            if (roleMenus.Count > 0)
            {
                List<string> menus = new List<string>();
                foreach (var item in roleMenus)
                {
                    menus.Add(_context.Menus.FirstOrDefault(u => u.MenuId == item.MenuId).Name);
                }
                return menus;
            }
            else
            {
                return new List<string>();
            }
        }
        [HttpPost]
        public bool viewNotes(ViewNotesViewModel model)
        {
            //int aspnetuserid = (int)_context.HttpContext.Session.GetInt32("AspNetUserId");
            //int aspnetuserid = (int)_context.HttpContext.Session.GetInt32("AspNetUserId");//

            try
            {
                RequestNote requestNote = _context.RequestNotes.FirstOrDefault(r => r.RequestId == model.RequestId);
                if (requestNote != null)
                {
                    requestNote.AdminNotes = model.Admin_Note;
                    requestNote.ModifiedDate = DateTime.Now;
                    _context.RequestNotes.Update(requestNote);
                    _context.SaveChanges();
                }
                else
                {
                    RequestNote newRequestNote = new RequestNote
                    {
                        RequestId = model.RequestId,
                        AdminNotes = model.Admin_Note,
                        CreatedDate = DateTime.Now,
                        CreatedBy = 2,
                    };
                    _context.RequestNotes.Add(newRequestNote);
                    _context.SaveChanges();
                }


                //var request2 = _httpContextAccessor.HttpContext.Request;
                //var token = request2.Cookies["jwt"];
                //CookieModel cookieModel = _jwtService.getDetails(token);
                //string AdminName = cookieModel.name;
                //AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
                //adminNavbarViewModel.AdminName = AdminName;
                //adminNavbarViewModel.Tab = 1;
                //int id = model.RequestId;
                //var PatientCancellationNotes = _context.RequestStatusLogs.FirstOrDefault(u => u.RequestId == id && u.Status == 7);
                //var AdminCancellationNotes = _context.RequestStatusLogs.FirstOrDefault(u => u.RequestId == id && u.Status == 6);
                //List<RequestStatusLog> requestStatusLogs = _context.RequestStatusLogs.Where(u => u.RequestId == id && u.Status == 2).ToList();
                //var Note = _context.RequestNotes.FirstOrDefault(u => u.RequestId == id);
                //model.adminNavbarViewModel = adminNavbarViewModel;
                //model.PatientCancellationNotes = PatientCancellationNotes?.Notes;
                //model.Admin_Cancellation_Note = AdminCancellationNotes?.Notes;
                //model.Admin_Note = Note.AdminNotes ?? "-";
                //model.Physician_Note = Note?.PhysicianNotes ?? "-";
                //model.Transfer_Notes = requestStatusLogs;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public ViewNotesViewModel viewNotes(int id)
        {
            var patientcancel = _context.RequestStatusLogs.FirstOrDefault(r => r.RequestId == id && r.Status == 7);
            var admincancel = _context.RequestStatusLogs.FirstOrDefault(r => r.RequestId == id && r.Status == 6);
            var transfernotes = _context.RequestStatusLogs.Where(r => r.RequestId == id && r.Status == 2).ToList();
            var requestnotes = _context.RequestNotes.FirstOrDefault(r => r.RequestId == id);

            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 1;

            ViewNotesViewModel viewnotesviewmodel = new ViewNotesViewModel
            {
                RequestId = id,
                Admin_Note = requestnotes?.AdminNotes ?? "-",
                Physician_Note = requestnotes?.PhysicianNotes ?? "-",
                Admin_Cancellation_Note = admincancel?.Notes,
                Cancellation_Note = patientcancel?.Notes,
                Transfer_Notes = transfernotes,
                adminNavbarViewModel = adminNavbarViewModel
            };
            return viewnotesviewmodel;
        }
        public ViewCaseModel viewCase(int id)
        {
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 1;

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
            viewCaseModel.RequestTypeId = data.RequestTypeId;
            viewCaseModel.region = data.RequestClient.State;
            viewCaseModel.adminNavbarViewModel = adminNavbarViewModel;
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
                requestor = "Concierge";
            }
            else
            {
                requestor = "Business";
            }
            viewCaseModel.Requestor = requestor;
            //var monthName = data.RequestClient.StrMonth;

            // Assuming data.RequestClient.StrMonth is a string containing the full month name

            //var month1 = data.RequestClient.StrMonth;
            //string monthName;
            //switch (month1)
            //{
            //    case "1":
            //        monthName = "January";
            //        break;
            //    case "2":
            //        monthName = "February";
            //        break;
            //    case "3":
            //        monthName = "March";
            //        break;
            //    case "4":
            //        monthName = "April";
            //        break;
            //    case "5":
            //        monthName = "May";
            //        break;
            //    case "6":
            //        monthName = "June";
            //        break;
            //    case "7":
            //        monthName = "July";
            //        break;
            //    case "8":
            //        monthName = "August";
            //        break;
            //    case "9":
            //        monthName = "September";
            //        break;
            //    case "10":
            //        monthName = "October";
            //        break;
            //    case "11":
            //        monthName = "November";
            //        break;
            //    case "12":
            //        monthName = "December";
            //        break;
            //    default:
            //        monthName = "Invalid month";
            //        break;
            //}
            //int month = DateTime.ParseExact(monthName, "MMMM", new CultureInfo("en-US")).Month;
            //viewCaseModel.DOB = new DateTime((int)data.RequestClient.IntYear, month, (int)data.RequestClient.IntDate);


            int month;//
            switch (data.RequestClient.StrMonth)
            {
                case "1":
                    month = 1;
                    break;
                case "2":
                    month = 2;
                    break;
                case "3":
                    month = 3;
                    break;
                case "4":
                    month = 4;
                    break;
                case "5":
                    month = 5;
                    break;
                case "6":
                    month = 6;
                    break;
                case "7":
                    month = 7;
                    break;
                case "8":
                    month = 8;
                    break;
                case "9":
                    month = 9;
                    break;
                case "10":
                    month = 10;
                    break;
                case "11":
                    month = 11;
                    break;
                case "12":
                    month = 12;
                    break;
                default:
                    month = 0;
                    break;
            }

            if (month == 0)
            {
                // Handle invalid month value here
            }
            else
            {
                viewCaseModel.DOB = new DateTime((int)data.RequestClient.IntYear, month, (int)data.RequestClient.IntDate);
            }
            return viewCaseModel;

        }
        public bool viewCase(ViewCaseModel model)
        {
            var requestId = model.RequestId;
            if (requestId != null)
            {
                var rid = _context.Requests.Where(u => u.RequestId == requestId).FirstOrDefault();
                var userToUpdate = _context.RequestClients.Where(u => u.RequestClientId == rid.RequestClientId).FirstOrDefault();
                if (userToUpdate != null)
                {
                    //userToUpdate.FirstName = model.FirstName;
                    //userToUpdate.LastName = model.LastName;
                    userToUpdate.PhoneNumber = model.PhoneNumber;
                    userToUpdate.Email = model.Email;
                    //userToUpdate.IntDate = model.DOB.Day;
                    //userToUpdate.IntYear = model.DOB.Year;
                    //userToUpdate.StrMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.DOB.Month);
                    _context.RequestClients.Update(userToUpdate);
                    _context.SaveChanges();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool viewCaseAssignModal(ViewCaseModel model)
        {
            int requestId = model.RequestId;
            if (requestId != null)
            {
                var requestToUpdate = _context.Requests.Include(u => u.Physician).Where(u => u.RequestId == requestId).FirstOrDefault();
                var PhysicianName = _context.Physicians.FirstOrDefault(u => u.PhysicianId == model.PhysicianId).FirstName;
                if (requestToUpdate != null)
                {
                    requestToUpdate.PhysicianId = model.PhysicianId;
                    requestToUpdate.ModifiedDate = DateTime.Now;
                    requestToUpdate.Status = 2;
                    _context.Requests.Update(requestToUpdate);
                    _context.SaveChanges();
                }
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.RequestId = requestId;
                requestStatusLog.Status = 2;
                requestStatusLog.Notes = "Admin transferred to Dr. " + PhysicianName + " on " + DateTime.Now.ToString("dd/MM/yyyy") + " at " + DateTime.Now.ToString("HH:mm:ss") + " : " + model.Description;
                requestStatusLog.CreatedDate = DateTime.Now;
                requestStatusLog.TransToPhysicianId = model.PhysicianId;
                _context.RequestStatusLogs.Add(requestStatusLog);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public MemoryStream downloadExcel()
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
            return memoryStream;
        }

        public MemoryStream Export(AdminDashboardTableView model)
        {
            try
            {
                var data = model.requests;
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Export");

                int count = 1;
                worksheet.Cell(1, count++).Value = "Name";
                if (model.status != "Unpaid")
                {
                    worksheet.Cell(1, count++).Value = "Date Of Birth";
                }
                if (model.status == "New" || model.status == "Pending" || model.status == "Active")
                {
                    worksheet.Cell(1, count++).Value = "Requestor";
                }
                if (model.status != "New")
                {
                    worksheet.Cell(1, count++).Value = "Physician Name";
                    worksheet.Cell(1, count++).Value = "Date Of Service";
                }
                if (model.status == "New")
                {
                    worksheet.Cell(1, count++).Value = "Requested Date";
                }
                if (model.status != "ToClose")
                {
                    worksheet.Cell(1, count++).Value = "Phone";
                }
                worksheet.Cell(1, count++).Value = "Address";
                if (model.status != "Conclude" || model.status != "Unpaid")
                {
                    worksheet.Cell(1, count++).Value = "Notes";
                }

                int row = 2;
                foreach (var item in data)
                {
                    count = 1;
                    var statusClass = "";
                    var dos = "";
                    var notes = "";
                    if (item.RequestTypeId == 1)
                    {
                        statusClass = "patient";
                    }
                    else if (item.RequestTypeId == 2)
                    {
                        statusClass = "family";
                    }
                    else if (item.RequestTypeId == 3)
                    {
                        statusClass = "concierge";
                    }
                    else
                    {
                        statusClass = "business";
                    }
                    foreach (var stat in item.RequestStatusLogs)
                    {
                        if (stat.Status == 2)
                        {
                            dos = stat.CreatedDate.ToString("MMMM dd,yyyy");
                            notes += stat.Notes + Environment.NewLine;
                        }
                    }
                    worksheet.Cell(row, count++).Value = string.Concat(item.RequestClient.FirstName, ',', item.RequestClient.LastName);
                    if (model.status != "Unpaid")
                    {

                        DateTime now = DateTime.Today;
                        int age = now.Year - DateTime.Parse($"{item.RequestClient.IntYear}-{item.RequestClient.StrMonth}-{item.RequestClient.IntDate}").Year;
                        if (DateTime.Parse($"{item.RequestClient.IntYear}-{item.RequestClient.StrMonth}-{item.RequestClient.IntDate}") > now.AddYears(-age))
                            age--;

                        worksheet.Cell(row, count++).Value = DateTime.Parse($"{item.RequestClient.IntYear}-{item.RequestClient.StrMonth}-{item.RequestClient.IntDate}").ToString("MMMM dd,yyyy") + "(" + age + ")";
                    }
                    if (model.status == "New" || model.status == "Pending" || model.status == "Active")
                    {
                        worksheet.Cell(row, count++).Value = statusClass.Substring(0, 1).ToUpper() + statusClass.Substring(1).ToLower() + " " + string.Concat(item.FirstName, ',', item.LastName);
                    }
                    if (model.status != "New")
                    {
                        worksheet.Cell(row, count++).Value = "Dr." + item.Physician == null ? "" : item.Physician.FirstName;
                    }
                    if (model.status == "New")
                    {

                        int hoursDifference = (int)(DateTime.Now - item.CreatedDate).TotalHours;
                        int minutesDifference = (DateTime.Now - item.CreatedDate).Minutes;

                        worksheet.Cell(row, count++).Value = item.CreatedDate.ToString("MMMM dd,yyyy") + " " + hoursDifference + "H " + minutesDifference + "M";
                    }
                    if (model.status != "New")
                    {
                        worksheet.Cell(row, count++).Value = item.AcceptedDate?.ToString("MMMM dd,yyyy");
                    }
                    if (model.status != "ToClose")
                    {
                        worksheet.Cell(row, count++).Value = item.RequestClient.PhoneNumber + "(Patient)" + (item.RequestTypeId != 2 ? item.PhoneNumber + "(" + statusClass.Substring(0, 1).ToUpper() + statusClass.Substring(1).ToLower() + ")" : "");
                    }
                    worksheet.Cell(row, count++).Value = (item.RequestClient.Address != null ? string.Concat(item.RequestClient.Address, ',', item.RequestClient.Street, ',', item.RequestClient.City, ',', item.RequestClient.State, ',', item.RequestClient.ZipCode) : string.Concat(item.RequestClient.Street, ',', item.RequestClient.City, ',', item.RequestClient.State, ',', item.RequestClient.ZipCode));
                    if (model.status != "Conclude" || model.status != "Unpaid")
                    {
                        worksheet.Cell(row, count++).Value = notes;
                    }
                    row++;
                }
                worksheet.Columns().AdjustToContents();

                var memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public bool sendLink(AdminDashboardTableView model)
        {
            var existingUser = _context.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
            var id = _context.Users.SingleOrDefault(u => u.Email == model.Email);
            bool userExists = true;
            if (existingUser != null)
            {
                //userExists = false;
                //aspNetUser.UserName = model.Email;
                //aspNetUser.Email = model.Email;
                //aspNetUser.PhoneNumber = model.PhoneNo;
                //aspNetUser.CreatedDate = DateTime.Now;
                //_context.AspNetUsers.Add(aspNetUser);
                //await _context.SaveChangesAsync();

                string senderEmail = "tatva.dotnet.karmadipsinhsolanki@outlook.com";
                string senderPassword = "Karmadips@2311";

                SmtpClient client = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };
                string email = model.Email;
                var userFirstName = model.FirstName + " " + model.LastName;
                var formatedDate = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                string resetLink = $"https://localhost:44339/Login/submitrequest";
                string message = $@"<html>
                                <body>  
                                <h1>Create Request for patient</h1>  
                                <h2>Hii {userFirstName},</h2>
                                <p style=""margin-top:30px;"">We have sent you link to create request for patient. So, please click the below link to create request:</p>
                                <p><a href=""{resetLink}"">Create Request</a></p> 
                                <p>If you don't need request creation then please ignore this mail.</p>
                                </body>
                                </html>";
                if (email != null)
                {
                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, "HalloDoc"),
                        Subject = "Register Case",
                        IsBodyHtml = true,
                        Body = message,
                    };
                    mailMessage.To.Add(email);
                    client.Send(mailMessage);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public int createRequest(CreateRequestViewModel model)
        {
            AspNetUser aspNetUser = new AspNetUser();
            User user = new User();
            Request request = new Request();
            RequestClient requestClient = new RequestClient();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();

            var region = _context.Regions.FirstOrDefault(u => u.Name.Trim().ToLower().Replace(" ", "") == model.State.Trim().ToLower().Replace(" ", ""));
            if (region == null)
            {
                return 0;
            }
            var blockedUser = _context.BlockRequests.FirstOrDefault(u => u.Email == model.Email);
            if (blockedUser != null)
            {
                return 1;
            }

            var existingUser = _context.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
            var id = _context.Users.SingleOrDefault(u => u.Email == model.Email);
            bool userExists = true;
            if (existingUser == null)
            {
                userExists = false;
                aspNetUser.UserName = model.Email;
                aspNetUser.Email = model.Email;
                aspNetUser.PhoneNumber = model.PhoneNumber;
                aspNetUser.CreatedDate = DateTime.Now;
                _context.AspNetUsers.Add(aspNetUser);
                _context.SaveChanges();

                user.AspNetUserId = aspNetUser.Id;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Mobile = model.PhoneNumber;
                user.Street = model.Street;
                user.City = model.City;
                user.State = model.State;
                user.ZipCode = model.ZipCode;
                user.IntDate = model.DOB.Day;
                user.StrMonth = model.DOB.Month.ToString();
                user.IntYear = model.DOB.Year;
                user.CreatedBy = aspNetUser.Id;
                user.CreatedDate = DateTime.Now;
                _context.Users.Add(user);
                _context.SaveChanges();
                string senderEmail = "tatva.dotnet.karmadipsinhsolanki@outlook.com";
                string senderPassword = "Karmadips@2311";

                SmtpClient client = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };
                string email = model.Email;
                var userFirstName = model.FirstName + " " + model.LastName;
                var formatedDate = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                string resetLink = $"https://localhost:44339/PatientRequest/CreatePassword?email={email}";
                string message = $@"<html>
                                <body>  
                                <h1>Create password request</h1>  
                                <h2>Hii {userFirstName},</h2>
                                <p style=""margin-top:30px;"">We have received an account creation request on {formatedDate}. So,in order to create your account we need your password,so please click the below link to create password:</p>
                                <p><a href=""{resetLink}"">Create Password</a></p> 
                                <p>If you didn't request an account creation then please ignore this mail.</p>
                                </body>
                                </html>";
                if (email != null)
                {
                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, "HalloDoc"),
                        Subject = "Create password request",
                        IsBodyHtml = true,
                        Body = message,
                    };
                    mailMessage.To.Add(email);
                    client.Send(mailMessage);
                }
            }

            requestClient.FirstName = model.FirstName;
            requestClient.LastName = model.LastName;
            requestClient.PhoneNumber = model.PhoneNumber;
            requestClient.Location = model.City;
            requestClient.Address = model.Street;
            requestClient.RegionId = region.RegionId;
            if (model.AdminNotes != null)
            {
                requestClient.Notes = model.AdminNotes;
            }
            requestClient.Email = model.Email;
            requestClient.IntDate = model.DOB.Day;
            requestClient.StrMonth = model.DOB.Month.ToString();
            requestClient.IntYear = model.DOB.Year;
            requestClient.Street = model.Street;
            requestClient.City = model.City;
            requestClient.State = model.State;
            requestClient.ZipCode = model.ZipCode;
            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

            int requests = _context.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();
            string ConfirmationNumber = string.Concat(region.Abbreviation, model.FirstName.Substring(0, 2).ToUpper(), model.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));
            request.RequestTypeId = 4;
            if (!userExists)
            {
                request.UserId = user.UserId;
            }
            else
            {
                request.UserId = id.UserId;
            }
            request.FirstName = model.FirstName;
            request.LastName = model.LastName;
            request.Email = model.Email;
            request.PhoneNumber = model.PhoneNumber;
            request.Status = 1;
            request.CreatedDate = DateTime.Now;
            request.RequestClientId = requestClient.RequestClientId;
            request.ConfirmationNumber = ConfirmationNumber;
            _context.Requests.Add(request);
            _context.SaveChanges();

            requestStatusLog.RequestId = request.RequestId;
            requestStatusLog.Status = 1;
            requestStatusLog.Notes = model.AdminNotes;
            requestStatusLog.CreatedDate = DateTime.Now;
            _context.RequestStatusLogs.Add(requestStatusLog);
            _context.SaveChanges();
            return 2;
        }
        public bool verifyState(string? region)
        {
            //var details = _context.Requests.FirstOrDefault(i => i.RequestId == model.RequestId);
            Region state = _context.Regions.FirstOrDefault(u => u.Name.Trim().ToLower().Replace(" ", "") == region.Trim().ToLower().Replace(" ", ""));
            if (state != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public CreateRequestViewModel createRequest()
        {
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            CreateRequestViewModel createRequestViewModel = new CreateRequestViewModel();
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 1;
            createRequestViewModel.adminNavbarViewModel = adminNavbarViewModel;
            return createRequestViewModel;
        }
        public HealthProfessional fetchBusinessDetail(int id)
        {
            return _context.HealthProfessionals.FirstOrDefault(r => r.VendorId == id);
        }
        public List<HealthProfessional> fetchBusiness(int id)
        {
            return _context.HealthProfessionals.Where(r => r.Profession == id).ToList();
        }

        public List<Region> fetchRegions()
        {
            return _context.Regions.ToList(); // Directly project to List<Region>
        }
        List<Physician> IAdmin.fetchPhysicians(int id)
        {
            BitArray isDeleted = new BitArray(1);
            isDeleted.Set(0, false);

            List<Physician> physicians = _context.PhysicianRegions
        .Where(pr => pr.RegionId == id && pr.Physician.IsDeleted == isDeleted) // Filter by region and active status
        .Select(pr => pr.Physician) // Select only the Physician object
        .ToList();
            return physicians;
        }
        public List<CaseTag> fetchTags()
        {
            return _context.CaseTags.ToList();
        }
        public bool assignCase(AdminDashboardTableView model)
        {
            int requestId = model.RequestId;
            if (requestId != null)
            {
                var requestToUpdate = _context.Requests.Include(u => u.Physician).Where(u => u.RequestId == requestId).FirstOrDefault();
                var PhysicianName = _context.Physicians.FirstOrDefault(u => u.PhysicianId == model.PhysicianId).FirstName;
                if (requestToUpdate != null)
                {
                    requestToUpdate.PhysicianId = model.PhysicianId;
                    requestToUpdate.ModifiedDate = DateTime.Now;
                    requestToUpdate.Status = 2;
                    _context.Requests.Update(requestToUpdate);
                    _context.SaveChanges();
                }
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.RequestId = requestId;
                requestStatusLog.Status = 2;
                requestStatusLog.Notes = "Admin transferred to Dr. " + PhysicianName + " on " + DateTime.Now.ToString("dd/MM/yyyy") + " at " + DateTime.Now.ToString("HH:mm:ss") + " : " + model.Description;
                requestStatusLog.CreatedDate = DateTime.Now;
                requestStatusLog.TransToPhysicianId = model.PhysicianId;
                _context.RequestStatusLogs.Add(requestStatusLog);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool transferCase(AdminDashboardTableView model)
        {
            int requestId = model.RequestId;
            if (requestId != null)
            {
                var requestToUpdate = _context.Requests.Include(u => u.Physician).Where(u => u.RequestId == requestId).FirstOrDefault();
                var PhysicianName = _context.Physicians.FirstOrDefault(u => u.PhysicianId == model.PhysicianId).FirstName;
                if (requestToUpdate != null)
                {
                    requestToUpdate.PhysicianId = model.PhysicianId;
                    requestToUpdate.ModifiedDate = DateTime.Now;
                    requestToUpdate.Status = 2;
                    _context.Requests.Update(requestToUpdate);
                    _context.SaveChanges();
                }
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.RequestId = requestId;
                requestStatusLog.Status = 2;
                requestStatusLog.Notes = "Admin transferred to Dr. " + PhysicianName + " on " + DateTime.Now.ToString("dd/MM/yyyy") + " at " + DateTime.Now.ToString("HH:mm:ss") + " : " + model.Description;
                requestStatusLog.CreatedDate = DateTime.Now;
                requestStatusLog.TransToPhysicianId = model.PhysicianId;
                _context.RequestStatusLogs.Add(requestStatusLog);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool cancelCase(AdminDashboardTableView model)
        {
            int requestId = model.RequestId;
            if (requestId != null)
            {
                var requestToUpdate = _context.Requests.Where(u => u.RequestId == requestId).FirstOrDefault();

                if (requestToUpdate != null)
                {
                    requestToUpdate.Status = 6;
                    requestToUpdate.CaseTag = model.CaseTagId;
                    _context.Requests.Update(requestToUpdate);
                    _context.SaveChanges();
                }
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.RequestId = requestId;
                requestStatusLog.Status = 6;
                requestStatusLog.Notes = model.CancelDescription;
                requestStatusLog.CreatedDate = DateTime.Now;
                _context.RequestStatusLogs.Add(requestStatusLog);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool clearCase(AdminDashboardTableView model)
        {
            int requestId = model.RequestId;
            if (requestId != null)
            {
                var requestToUpdate = _context.Requests.Where(u => u.RequestId == requestId).FirstOrDefault();

                if (requestToUpdate != null)
                {
                    requestToUpdate.Status = 10;
                    _context.Requests.Update(requestToUpdate);
                    _context.SaveChanges();
                }
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.RequestId = (int)requestId;
                requestStatusLog.Status = 10;
                requestStatusLog.CreatedDate = DateTime.Now;
                _context.RequestStatusLogs.Add(requestStatusLog);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool blockCase(AdminDashboardTableView model)
        {
            int requestId = model.RequestId;
            if (requestId != null)
            {
                var requestToUpdate = _context.Requests.Include(r => r.RequestClient).Where(u => u.RequestId == requestId).FirstOrDefault();

                if (requestToUpdate != null)
                {
                    requestToUpdate.Status = 11;
                    requestToUpdate.ModifiedDate = DateTime.Now;
                    _context.Requests.Update(requestToUpdate);
                    _context.SaveChanges();
                }
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.RequestId = requestId;
                requestStatusLog.Status = 11;
                requestStatusLog.Notes = model.BlockReason;
                requestStatusLog.CreatedDate = DateTime.Now;
                _context.RequestStatusLogs.Add(requestStatusLog);
                _context.SaveChanges();

                BlockRequest blockRequest = new BlockRequest();
                blockRequest.RequestId = model.RequestId;
                blockRequest.Reason = model.BlockReason;
                blockRequest.Email = requestToUpdate.Email;
                blockRequest.PhoneNumber = requestToUpdate.PhoneNumber;
                blockRequest.CreatedDate = DateTime.Now;
                blockRequest.IsActive = new BitArray(new[] { false });
                //blockRequest.FirstName = requestToUpdate.RequestClient.FirstName;

                _context.BlockRequests.Add(blockRequest);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// this controller is for view upload which contain get and post method of view upload and action methods of delete and send mail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool deleteViewUploadFile(string fileids)
        {
            if (fileids != null)
            {
                string[] deleteFileIds = fileids.Split(',');
                BitArray check = new BitArray(1);
                check.Set(0, true);
                for (int i = 0; i < deleteFileIds.Length; i++)
                {
                    int n = i;
                    RequestWiseFile requestWiseFile = _context.RequestWiseFiles.FirstOrDefault(i => i.RequestWiseFileId == Convert.ToInt32(deleteFileIds[n]));
                    requestWiseFile.IsDeleted = check;
                    _context.RequestWiseFiles.Update(requestWiseFile);
                    _context.SaveChanges();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool mailDocument(List<int> requestFilesId, int requestId)
        {
            string senderEmail = "tatva.dotnet.karmadipsinhsolanki@outlook.com";
            string senderPassword = "Karmadips@2311";

            SmtpClient client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            var request = _context.Requests.FirstOrDefault(u => u.RequestId == requestId);
            string? email = _context.RequestClients.FirstOrDefault(x => x.RequestClientId == request.RequestClientId).Email;
            var requestWiseFile = from requestFiles in _context.RequestWiseFiles
                                  where requestFilesId.Contains(requestFiles.RequestWiseFileId)
                                  select new RequestWiseFile
                                  {
                                      RequestWiseFileId = requestFiles.RequestWiseFileId,
                                      FileName = requestFiles.FileName,
                                      RequestId = requestFiles.RequestId,
                                  };
            string message = $@"<html>
                                <body>  
                                <h1>All Documents</h1>
                                </body>
                                </html>";
            if (email != null)
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "HalloDoc"),
                    Subject = "Documents",
                    Body = message,
                    IsBodyHtml = true
                };
                foreach (var item in requestWiseFile)
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/" + item.FileName);
                    Attachment attachment = new Attachment(filePath);
                    mailMessage.Attachments.Add(attachment);
                }
                mailMessage.To.Add(email);
                client.Send(mailMessage);
                EmailLog emailLog = new EmailLog();
                emailLog.SubjectName = mailMessage.Subject;
                emailLog.EmailId = email;
                emailLog.ConfirmationNumber = request.ConfirmationNumber;
                emailLog.CreateDate = DateTime.Now;
                _context.EmailLogs.Add(emailLog);
                _context.SaveChanges();
                foreach (var attachment in mailMessage.Attachments)
                {
                    attachment.Dispose();
                }
                return true;
            }
            return false;
        }
        public ViewUploadViewModel viewUpload(int id)
        {
            //to save file in wwwroot,that is uploaded by patient
            //token
            BitArray check = new BitArray(1);
            check.Set(0, false);
            var request = _context.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            var documents = _context.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id && u.IsDeleted == check).ToList();
            var user = _context.Users.FirstOrDefault(u => u.UserId == request.UserId);
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 1;
            ViewUploadViewModel viewUploadViewModel = new ViewUploadViewModel();
            viewUploadViewModel.patient_name = string.Concat(request.RequestClient.FirstName, ' ', request.RequestClient.LastName);
            //viewUploadViewModel.name = string.Concat(user.FirstName, ' ', user.LastName);        uncomment
            viewUploadViewModel.confirmation_number = request.ConfirmationNumber;
            viewUploadViewModel.requestWiseFiles = documents;
            //viewUploadViewModel.uploader_name = string.Concat(request.FirstName, ' ', request.LastName);
            viewUploadViewModel.RequestId = id;
            viewUploadViewModel.adminNavbarViewModel = adminNavbarViewModel;
            return viewUploadViewModel;
        }
        public bool viewUpload(ViewUploadViewModel model)
        {
            if (model.File != null && model.File.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", model.File.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    model.File.CopyTo(stream);
                }
                RequestWiseFile requestWiseFile = new RequestWiseFile();
                BitArray check = new BitArray(1);
                check.Set(0, false);
                requestWiseFile.RequestId = (int)model?.RequestId;
                //IformFile has a property that to store filepath u need to add .filename behind it to store path
                //requestWiseFile.AdminId = 1;
                requestWiseFile.FileName = model.File.FileName;
                requestWiseFile.IsDeleted = check;
                requestWiseFile.CreatedDate = DateTime.Now;
                _context.RequestWiseFiles.Add(requestWiseFile);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// this controller is for close case ,which contains closecase get method,closeCaseSaveBtn post method and closeCaseBtn action method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewUploadViewModel closeCase(int id)
        {
            var details = _context.Requests.Include(u => u.RequestClient).FirstOrDefault(i => i.RequestId == id);
            var month1 = details.RequestClient.StrMonth;
            string monthName = monthNameFunc(month1);
            int month = DateTime.ParseExact(monthName, "MMMM", new CultureInfo("en-US")).Month;
            var documents = _context.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id).ToList();
            var user = _context.Users.FirstOrDefault(u => u.UserId == details.UserId);
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 1;
            ViewUploadViewModel viewUploadViewModel = new ViewUploadViewModel();
            viewUploadViewModel.RequestId = id;
            viewUploadViewModel.DOB = new DateTime((int)details.RequestClient.IntYear, month, (int)details.RequestClient.IntDate);
            viewUploadViewModel.FirstName = details.RequestClient.FirstName;
            viewUploadViewModel.LastName = details.RequestClient.LastName;
            viewUploadViewModel.PhoneNumber = details.RequestClient.PhoneNumber;
            viewUploadViewModel.Email = details.RequestClient.Email;
            viewUploadViewModel.patient_name = string.Concat(details.RequestClient.FirstName, ' ', details.RequestClient.LastName);
            viewUploadViewModel.name = string.Concat(user?.FirstName, ' ', user?.LastName);
            viewUploadViewModel.confirmation_number = details.ConfirmationNumber;
            viewUploadViewModel.requestWiseFiles = documents;
            viewUploadViewModel.adminNavbarViewModel = adminNavbarViewModel;
            return viewUploadViewModel;

            //to save file in wwwroot,that is uploaded by patient
            //token
            //var request = _context.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            //var documents = _context.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id).ToList();
            //var user = _context.Users.FirstOrDefault(u => u.UserId == request.UserId);
            //var request2 = _httpContextAccessor.HttpContext.Request;
            //var token = request2.Cookies["jwt"];
            //CookieModel cookieModel = _jwtService.getDetails(token);
            //string AdminName = cookieModel.name;

            //AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            //adminNavbarViewModel.AdminName = AdminName;
            //adminNavbarViewModel.Tab = 1;
            //ViewUploadViewModel viewUploadViewModel = new ViewUploadViewModel();
            //var data = _context.Requests.Include(u => u.RequestClient).FirstOrDefault(u => u.RequestId == id);
            //viewUploadViewModel.Email = data?.RequestClient?.Email;
            //viewUploadViewModel.PhoneNumber = data.RequestClient.PhoneNumber;
            //viewUploadViewModel.FirstName = data.RequestClient.FirstName;
            //viewUploadViewModel.LastName = data.RequestClient.LastName;
            //viewUploadViewModel.adminNavbarViewModel = adminNavbarViewModel;

            //int month;
            //switch (data.RequestClient.StrMonth)
            //{
            //    case "1":
            //        month = 1;
            //        break;
            //    case "2":
            //        month = 2;
            //        break;
            //    case "3":
            //        month = 3;
            //        break;
            //    case "4":
            //        month = 4;
            //        break;
            //    case "5":
            //        month = 5;
            //        break;
            //    case "6":
            //        month = 6;
            //        break;
            //    case "7":
            //        month = 7;
            //        break;
            //    case "8":
            //        month = 8;
            //        break;
            //    case "9":
            //        month = 9;
            //        break;
            //    case "10":
            //        month = 10;
            //        break;
            //    case "11":
            //        month = 11;
            //        break;
            //    case "12":
            //        month = 12;
            //        break;
            //    default:
            //        month = 0;
            //        break;
            //}

            //if (month == 0)
            //{
            //    // Handle invalid month value here
            //}
            //else
            //{
            //    viewUploadViewModel.DOB = new DateTime((int)data.RequestClient.IntYear, month, (int)data.RequestClient.IntDate);
            //}
            //viewUploadViewModel.patient_name = string.Concat(request.RequestClient.FirstName, ' ', request.RequestClient.LastName);
            //viewUploadViewModel.name = string.Concat(user?.FirstName, ' ', user?.LastName);
            //viewUploadViewModel.confirmation_number = request.ConfirmationNumber;
            //viewUploadViewModel.requestWiseFiles = documents;
            //viewUploadViewModel.RequestId = id;
            //return viewUploadViewModel;
        }
        public bool closeCaseBtn(int requestId)
        {
            Request request = _context.Requests.FirstOrDefault(i => i.RequestId == requestId);
            if (request != null)
            {
                request.Status = 9;
                request.ModifiedDate = DateTime.Now;
                _context.Requests.Update(request);
                _context.SaveChanges();
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.RequestId = requestId;
                requestStatusLog.CreatedDate = DateTime.Now;
                requestStatusLog.Status = 9;
                _context.RequestStatusLogs.Add(requestStatusLog);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool closeCaseSaveBtn(ViewUploadViewModel model)
        {
            var details = _context.Requests.FirstOrDefault(i => i.RequestId == model.RequestId);
            RequestClient requestClientDetail = _context.RequestClients.FirstOrDefault(i => i.RequestClientId == details.RequestClientId);
            if (requestClientDetail != null)
            {
                requestClientDetail.Email = model?.Email;
                requestClientDetail.PhoneNumber = model?.PhoneNumber;
                _context.RequestClients.Update(requestClientDetail);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
            //int requestId = (int)model.RequestId;
            //if (requestId != null)
            //{
            //var requestToUpdate = _context.Requests.Where(u => u.RequestId == requestId).FirstOrDefault();

            //if (requestToUpdate != null)
            //{
            //    requestToUpdate.Status = 9;
            //    requestToUpdate.PhoneNumber = model.PhoneNumber;
            //    requestToUpdate.Email = model.Email;
            //    _context.Requests.Update(requestToUpdate);
            //    _context.SaveChanges();
            //}
            //RequestStatusLog requestStatusLog = new RequestStatusLog();
            //requestStatusLog.RequestId = (int)requestId;
            //requestStatusLog.Status = 9;
            //requestStatusLog.CreatedDate = DateTime.Now;
            //_context.RequestStatusLogs.Add(requestStatusLog);
            //_context.SaveChanges();

            //if (model.File != null && model.File.Length > 0)
            //{
            //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", model.File.FileName);
            //    using (var stream = System.IO.File.Create(filePath))
            //    {
            //        model.File.CopyTo(stream);
            //    }
            //    RequestWiseFile requestWiseFile = new RequestWiseFile();
            //    requestWiseFile.RequestId = (int)model.RequestId;
            //    //IformFile has a property that to store filepath u need to add .filename behind it to store path
            //    //requestWiseFile.AdminId = 1;
            //    requestWiseFile.FileName = model.File.FileName;
            //    requestWiseFile.CreatedDate = DateTime.Now;
            //    _context.RequestWiseFiles.Add(requestWiseFile);
            //    _context.SaveChanges();
            //}
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

        }

        //bool closeCase(AdminDashboardTableView model)
        //{
        //    int requestId = model.RequestId;
        //    if (requestId != null)
        //    {
        //        var requestToUpdate = _context.Requests.Where(u => u.RequestId == requestId).FirstOrDefault();

        //        if (requestToUpdate != null)
        //        {
        //            requestToUpdate.Status = 9;
        //            _context.Requests.Update(requestToUpdate);
        //            _context.SaveChanges();
        //        }
        //        RequestStatusLog requestStatusLog = new RequestStatusLog();
        //        requestStatusLog.RequestId = (int)requestId;
        //        requestStatusLog.Status = 9;
        //        requestStatusLog.CreatedDate = DateTime.Now;
        //        _context.RequestStatusLogs.Add(requestStatusLog);
        //        _context.SaveChanges();
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}


        public OrderViewModel SendOrder(int id)
        {
            var healthProfessionalType = _context.HealthProfessionalTypes.ToList();
            var healthProfessional = _context.HealthProfessionals.ToList();

            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 1;

            OrderViewModel orderViewModel = new OrderViewModel();
            orderViewModel.RequestId = id;
            orderViewModel.healthProfessionalTypes = healthProfessionalType;
            orderViewModel.healthProfessionals = healthProfessional;
            orderViewModel.adminNavbarViewModel = adminNavbarViewModel;
            return orderViewModel;
        }
        public bool SendOrder(OrderViewModel model)
        {
            try
            {
                var request2 = _httpContextAccessor.HttpContext.Request;
                var token = request2.Cookies["jwt"];
                CookieModel cookieModel = _jwtService.getDetails(token);
                int id = cookieModel.userId;
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.FaxNumber = model.FaxNumber;
                orderDetail.VendorId = model.VendorId;
                orderDetail.NoOfRefill = (int)model.NoOfRefill;
                orderDetail.Prescription = model.Prescription;
                orderDetail.RequestId = model.RequestId;
                orderDetail.CreatedDate = DateTime.Now;
                orderDetail.Email = model.Email;
                orderDetail.CreatedBy = id.ToString();
                orderDetail.BusinessContact = model.BusinessContact;
                _context.OrderDetails.Add(orderDetail);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //current
        public bool sendAgreement(AdminDashboardTableView model)
        {
            AdminDashboardTableView adminDashboardTableView = new AdminDashboardTableView();
            var data = _context.Requests.Include(u => u.RequestClient).FirstOrDefault(u => u.RequestId == model.RequestId);

            var existingUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);
            //var id = _context.Users.SingleOrDefault(u => u.Email == model.Email);
            bool userExists = true;
            if (existingUser != null)
            {
                var userId = _context.Requests.Include(u => u.RequestClient).FirstOrDefault(u => u.RequestId == model.RequestId);
                string senderEmail = "tatva.dotnet.karmadipsinhsolanki@outlook.com";
                string senderPassword = "Karmadips@2311";

                SmtpClient client = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };
                string email = model.Email;
                var aspnetUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == email);
                var userFirstName = data.RequestClient.FirstName;
                var requestId = (int)userId.RequestId;
                var formatedDate = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                string reviewAgreement = $"https://localhost:44339/Login/ReviewAgreement?name={userFirstName}&email={email}&requestId={requestId}";
                string message = $@"<html>
                                <body>  
                                <h1>Review Agreement</h1>  
                                <h2>Hii {userFirstName},</h2>
                                <p style=""margin-top:30px;"">We have sent you link to review the agreement. So, please click the below link to read agreement:</p>
                                <p><a href=""{reviewAgreement}"">Review Agreement here</a></p> 
                                </body>
                                </html>";
                if (aspnetUser != null)
                {
                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, "HalloDoc"),
                        Subject = "Review Agreement",
                        IsBodyHtml = true,
                        Body = message,
                    };
                    mailMessage.To.Add(email);
                    client.Send(mailMessage);
                    EmailLog emailLog = new EmailLog();
                    emailLog.SubjectName = mailMessage.Subject;
                    emailLog.EmailId = email;
                    emailLog.ConfirmationNumber = userId?.ConfirmationNumber;
                    emailLog.CreateDate = DateTime.Now;
                    _context.EmailLogs.Add(emailLog);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //bool sendAgreement(AdminDashboardTableView model)
        //{

        //}

        //bool sendLink(AdminDashboardTableView model)
        //{
        //    var existingUser = _context.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
        //    var id = _context.Users.SingleOrDefault(u => u.Email == model.Email);
        //    bool userExists = true;
        //    if (existingUser != null)
        //    {
        //        //userExists = false;
        //        //aspNetUser.UserName = model.Email;
        //        //aspNetUser.Email = model.Email;
        //        //aspNetUser.PhoneNumber = model.PhoneNo;
        //        //aspNetUser.CreatedDate = DateTime.Now;
        //        //_context.AspNetUsers.Add(aspNetUser);
        //        //await _context.SaveChangesAsync();

        //        string senderEmail = "tatva.dotnet.karmadipsinhsolanki@outlook.com";
        //        string senderPassword = "Karmadips@2311";

        //        SmtpClient client = new SmtpClient("smtp.office365.com")
        //        {
        //            Port = 587,
        //            Credentials = new NetworkCredential(senderEmail, senderPassword),
        //            EnableSsl = true,
        //            DeliveryMethod = SmtpDeliveryMethod.Network,
        //            UseDefaultCredentials = false
        //        };
        //        string email = model.Email;
        //        var userFirstName = model.FirstName + " " + model.LastName;
        //        var formatedDate = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        //        string resetLink = $"https://localhost:44339/Login/submitrequest";
        //        string message = $@"<html>
        //                        <body>  
        //                        <h1>Create Request for patient</h1>  
        //                        <h2>Hii {userFirstName},</h2>
        //                        <p style=""margin-top:30px;"">We have sent you link to create request for patient. So, please click the below link to create request:</p>
        //                        <p><a href=""{resetLink}"">Create Request</a></p> 
        //                        <p>If you don't need request creation then please ignore this mail.</p>
        //                        </body>
        //                        </html>";
        //        if (email != null)
        //        {
        //            MailMessage mailMessage = new MailMessage
        //            {
        //                From = new MailAddress(senderEmail, "HalloDoc"),
        //                Subject = "Register Case",
        //                IsBodyHtml = true,
        //                Body = message,
        //            };
        //            mailMessage.To.Add(email);
        //            client.Send(mailMessage);
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        ////
        public string monthNameFunc(string month1)
        {
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
            return monthName;
        }
        public EncounterViewModel encounter(int id)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 1;
            var details = _context.EncounterForms.FirstOrDefault(r => r.RequestId == id);
            var clientDetails = _context.Requests.Include(u => u.RequestClient).FirstOrDefault(r => r.RequestId == id);
            var month1 = clientDetails?.RequestClient?.StrMonth;

            string monthName = monthNameFunc(month1);
            int month = DateTime.ParseExact(monthName, "MMMM", new CultureInfo("en-US")).Month;

            EncounterViewModel encounterForm = new EncounterViewModel()
            {
                FirstName = clientDetails?.RequestClient?.FirstName,
                LastName = clientDetails?.RequestClient?.LastName,
                Location = clientDetails?.RequestClient?.Address,
                PhoneNumber = clientDetails?.RequestClient?.PhoneNumber,
                Email = clientDetails?.RequestClient?.Email,
                DOB = new DateTime((int)clientDetails.RequestClient.IntYear, month, (int)clientDetails.RequestClient.IntDate),
                Date = details?.Date ?? DateTime.Now,
                HistoryOfPresentIllness = details?.HistoryIllness,
                MedicalHistory = details?.MedicalHistory,
                Medications = details?.Medications,
                Allergies = details?.Allergies,
                temp = details?.Temp,
                HR = details?.Hr,
                RR = details?.Rr,
                BPSystolic = details?.BpS,
                BPDiastolic = details?.BpD,
                O2 = details?.O2,
                Pain = details?.Pain,
                Heent = details?.Heent,
                Chest = details?.Chest,
                CV = details?.Cv,
                ABD = details?.Abd,
                Extr = details?.Extr,
                Skin = details?.Skin,
                Neuro = details?.Neuro,
                Other = details?.Other,
                Diagnosis = details?.Diagnosis,
                TreatmentPlan = details?.TreatmentPlan,
                MedicationsDispensed = details?.MedicationDispensed,
                adminNavbarViewModel = adminNavbarViewModel,
                Procedures = details?.Procedures,
                Followup = details?.FollowUp,
                RequestId = id,
            };
            return encounterForm;
        }
        public bool encounter(EncounterViewModel model)
        {
            if (model.RequestId != null)
            {
                var details = _context.EncounterForms.FirstOrDefault(r => r.RequestId == model.RequestId);
                if (details == null)
                {
                    EncounterForm encounterForm = new EncounterForm();
                    encounterForm.RequestId = (int)model.RequestId;
                    encounterForm.Date = model?.Date ?? DateTime.Now;
                    encounterForm.HistoryIllness = model?.HistoryOfPresentIllness;
                    encounterForm.MedicalHistory = model?.MedicalHistory;
                    encounterForm.Medications = model?.Medications;
                    encounterForm.Allergies = model?.Allergies;
                    encounterForm.Temp = model?.temp;
                    encounterForm.Hr = model?.HR;
                    encounterForm.Rr = model?.RR;
                    encounterForm.BpS = model?.BPSystolic;
                    encounterForm.BpD = model?.BPDiastolic;
                    encounterForm.O2 = model?.O2;
                    encounterForm.Pain = model?.Pain;
                    encounterForm.Heent = model?.Heent;
                    encounterForm.Chest = model?.Chest;
                    encounterForm.Abd = model?.ABD;
                    encounterForm.Extr = model?.Extr;
                    encounterForm.Skin = model?.Skin;
                    encounterForm.Neuro = model?.Neuro;
                    encounterForm.Other = model?.Other;
                    encounterForm.Cv = model?.CV;
                    encounterForm.Diagnosis = model?.Diagnosis;
                    encounterForm.TreatmentPlan = model?.TreatmentPlan;
                    encounterForm.MedicationDispensed = model?.MedicationsDispensed;
                    encounterForm.Procedures = model?.Procedures;
                    encounterForm.FollowUp = model?.Followup;
                    _context.EncounterForms.Add(encounterForm);
                    _context.SaveChanges();
                }
                else
                {
                    var detail = _context.EncounterForms.FirstOrDefault(r => r.RequestId == model.RequestId);
                    detail.Date = model?.Date ?? DateTime.Now;
                    detail.HistoryIllness = model?.HistoryOfPresentIllness;
                    detail.MedicalHistory = model?.MedicalHistory;
                    detail.Medications = model?.Medications;
                    detail.Allergies = model?.Allergies;
                    detail.Temp = model?.temp;
                    detail.Hr = model?.HR;
                    detail.Rr = model?.RR;
                    detail.BpS = model?.BPSystolic;
                    detail.BpD = model?.BPDiastolic;
                    detail.O2 = model?.O2;
                    detail.Pain = model?.Pain;
                    detail.Heent = model?.Heent;
                    detail.Chest = model?.Chest;
                    detail.Abd = model?.ABD;
                    detail.Extr = model?.Extr;
                    detail.Skin = model?.Skin;
                    detail.Neuro = model?.Neuro;
                    detail.Other = model?.Other;
                    detail.Cv = model?.CV;
                    detail.Diagnosis = model?.Diagnosis;
                    detail.TreatmentPlan = model?.TreatmentPlan;
                    detail.MedicationDispensed = model?.MedicationsDispensed;
                    detail.Procedures = model?.Procedures;
                    detail.FollowUp = model?.Followup;
                    _context.EncounterForms.Update(detail);
                    _context.SaveChanges();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        //adminprofile page
        public List<Region> fetchAdminRegions()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            int aspNetUserId = cookieModel.aspId;
            var AdminDetails = _context.Admins.FirstOrDefault(u => u.AspNetUserId == aspNetUserId);
            var regions = _context.AdminRegions.Include(u => u.Region).Where(u => u.AdminId == AdminDetails.AdminId).Select(ar => ar.RegionId).ToList();
            List<Region> regionName = (from regionNames in _context.Regions
                                       where regions.Contains(regionNames.RegionId)
                                       select new Region
                                       {
                                           RegionId = regionNames.RegionId,
                                           Name = regionNames.Name,
                                           Abbreviation = regionNames.Abbreviation
                                       }).ToList();
            return regionName;
        }
        public bool saveAdministratorDetail(AdminProfileViewModel model)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            int aspNetUserId = cookieModel.aspId;

            if (aspNetUserId != null)
            {
                HalloDoc.DataLayer.Models.Admin admin = _context.Admins.FirstOrDefault(u => u.AspNetUserId == aspNetUserId);
                admin.FirstName = model.FirstName;
                admin.LastName = model.LastName;
                admin.Email = model.Email;
                admin.Mobile = model.PhoneNumber1;
                _context.Admins.Update(admin);
                _context.SaveChanges();

                foreach (var adminRegion in _context.AdminRegions.Where(ar => ar.AdminId == admin.AdminId))
                {
                    _context.AdminRegions.Remove(adminRegion);
                }
                _context.SaveChanges();

                string selectedRegionString = model.SelectedRegion;

                if (!string.IsNullOrEmpty(selectedRegionString))
                {
                    string[] regionIds = selectedRegionString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var regionIdString in regionIds)
                    {
                        int regionId = int.Parse(regionIdString);
                        AdminRegion adminRegion = new AdminRegion();
                        adminRegion.RegionId = regionId;
                        adminRegion.AdminId = admin.AdminId;
                        _context.AdminRegions.Add(adminRegion);
                    }
                }

                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool saveBillingInformation(AdminProfileViewModel model)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            int aspNetUserId = cookieModel.aspId;
            if (aspNetUserId != null)
            {
                HalloDoc.DataLayer.Models.Admin admin = _context.Admins.FirstOrDefault(u => u.AspNetUserId == aspNetUserId);
                admin.Address1 = model.Address1;
                admin.Address2 = model.Address2;
                admin.City = model.city;
                admin.RegionId = model.StateId;
                admin.Zip = model.zip;
                admin.AltPhone = model.PhoneNumber2;
                _context.Admins.Update(admin);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool resetPassword(AdminProfileViewModel model)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            int aspNetUserId = cookieModel.aspId;
            if (aspNetUserId != null)
            {
                AspNetUser aspNetUser = _context.AspNetUsers.FirstOrDefault(u => u.Id == aspNetUserId);
                aspNetUser.PasswordHash = model.Password;
                _context.AspNetUsers.Update(aspNetUser);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public AdminProfileViewModel adminProfile()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            int aspNetUserId = cookieModel.aspId;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = cookieModel.name;
            adminNavbarViewModel.Tab = 3;
            var AdminDetails = _context.Admins.FirstOrDefault(u => u.AspNetUserId == aspNetUserId);
            var AspAdminDetail = _context.AspNetUsers.FirstOrDefault(u => u.Id == aspNetUserId);
            //var region = _context.Regions.ToList();
            //var regions = _context.AdminRegions.Include(u => u.Region).Where(u => u.AdminId == AdminDetails.AdminId).Select(ar => ar.RegionId).ToList();
            //List<Region> regionName = (from regionNames in _context.Regions where regions.Contains(regionNames.RegionId) select new Region
            //{
            //    RegionId = regionNames.RegionId,
            //    Name = regionNames.Name,
            //    Abbreviation = regionNames.Abbreviation
            //}).ToList();
            //var data = _context.Admins.Include(u => u.AspNetUsers).FirstOrDefault(u => u.AdminId == id);
            AdminProfileViewModel adminProfileViewModel = new AdminProfileViewModel();


            var allRegions = _context.Regions.ToList();

            List<AdminSelectedRegions> selectedRegions = allRegions.Select(r => new AdminSelectedRegions
            {
                IsSelected = _context.AdminRegions.Any(ar => ar.AdminId == AdminDetails.AdminId && ar.RegionId == r.RegionId),
                Name = r.Name,
                RegionId = r.RegionId
            })
            .ToList();

            //adminProfileViewModel.Password = data?.AspNetUsers.PasswordHash;
            //adminProfileViewModel.Status = data?.Status;
            //adminProfileViewModel.role = data?.Role;
            adminProfileViewModel.FirstName = AdminDetails.FirstName;
            adminProfileViewModel.LastName = AdminDetails.LastName;
            adminProfileViewModel.UserName = AspAdminDetail.UserName;
            adminProfileViewModel.Email = AdminDetails.Email;
            adminProfileViewModel.PhoneNumber1 = AdminDetails.Mobile;
            adminProfileViewModel.PhoneNumber2 = AdminDetails.AltPhone;
            adminProfileViewModel.Address1 = AdminDetails.Address1;
            adminProfileViewModel.Address2 = AdminDetails.Address2;
            adminProfileViewModel.city = AdminDetails.City;
            adminProfileViewModel.StateId = (int)AdminDetails.RegionId;
            adminProfileViewModel.State = selectedRegions;
            adminProfileViewModel.zip = AdminDetails.Zip;
            adminProfileViewModel.regions = allRegions;
            adminProfileViewModel.adminNavbarViewModel = adminNavbarViewModel;
            adminProfileViewModel.CreatedDate = AspAdminDetail.CreatedDate;
            return adminProfileViewModel;
        }
        public bool editAccess(EditAccessViewModel model)
        {
            List<RoleMenu> roleMenus = _context.RoleMenus.Where(i => i.RoleId == model.Id).ToList();
            string[] roleMenuItem = new string[roleMenus.Count];
            string[] modelRoleItems = model.RoleId.Split(',');

            for (var i = 0; i < roleMenus.Count; i++)
            {
                roleMenuItem[i] = roleMenus[i].MenuId.ToString();
            }
            for (var i = 0; i < modelRoleItems.Length; i++)
            {
                if (roleMenuItem.Contains(modelRoleItems[i]) == false)
                {
                    RoleMenu roleMenu = new RoleMenu();
                    roleMenu.MenuId = Convert.ToInt32(modelRoleItems[i]);
                    roleMenu.RoleId = model.Id;
                    _context.RoleMenus.Add(roleMenu);
                    _context.SaveChanges();
                }
            }
            for (var i = 0; i < roleMenuItem.Length; i++)
            {
                if (modelRoleItems.Contains(roleMenuItem[i]) == false)
                {
                    int n = i;
                    RoleMenu roleMenu = _context.RoleMenus.FirstOrDefault(i => i.RoleId == model.Id && i.MenuId == Convert.ToInt32(roleMenuItem[n]));
                    _context.RoleMenus.Remove(roleMenu);
                    _context.SaveChanges();
                }
            }
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            Role role = _context.Roles.FirstOrDefault(i => i.RoleId == model.Id);
            role.Name = model.RoleName;
            role.AccountType = (short)(model.AccountType == "admin-div" ? 2 : 1);
            role.ModifiedBy = cookieModel.name;
            role.ModifiedDate = DateTime.Now;
            _context.Roles.Update(role);
            _context.SaveChanges();
            return true;
        }
        public EditAccessViewModel editAccess(int id)
        {
            Role role = _context.Roles.FirstOrDefault(i => i.RoleId == id);
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 6;
            var allMenuItems = _context.Menus.ToList();
            List<MenuItem> menuItems = allMenuItems.Select(r => new MenuItem
            {
                IsSelected = _context.RoleMenus.Any(ar => ar.RoleId == role.RoleId && ar.MenuId == r.MenuId),
                Name = r.Name,
                MenuItemId = r.MenuId,
                AccountType = r.AccountType,
            })
            .ToList();
            EditAccessViewModel editAccessViewModel = new EditAccessViewModel();
            editAccessViewModel.adminNavbarViewModel = adminNavbarViewModel;
            editAccessViewModel.AccountType = role.AccountType == 2 ? "admin-div" : "provider-div";
            editAccessViewModel.Query = menuItems;
            editAccessViewModel.Id = id;
            editAccessViewModel.RoleName = role.Name;
            return editAccessViewModel;
        }
        public bool createAccess(CreateAccessViewModel model)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            Role role = new Role();
            role.CreatedBy = cookieModel.name;

            BitArray check = new BitArray(1);
            check.Set(0, false);
            role.IsDeleted = check;
            role.CreatedDate = DateTime.Now;
            role.AccountType = (short)(model.AccountType == "admin-div" ? 1 : 2);
            role.Name = model.RoleName;
            _context.Roles.Add(role);
            _context.SaveChanges();
            string selectedRoleString = model.RoleId;

            if (!string.IsNullOrEmpty(selectedRoleString))
            {
                string[] roleIds = selectedRoleString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var roleIdString in roleIds)
                {
                    int roleId = int.Parse(roleIdString);
                    RoleMenu roleMenu = new RoleMenu();
                    roleMenu.RoleId = role.RoleId;
                    roleMenu.MenuId = roleId;
                    _context.RoleMenus.Add(roleMenu);
                    _context.SaveChanges();
                }
            }
            return true;
        }
        public CreateAccessViewModel createAccess()
        {
            CreateAccessViewModel createAccessViewModel = new CreateAccessViewModel();
            List<Menu> menu = _context.Menus.ToList();
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 6;
            createAccessViewModel.adminNavbarViewModel = adminNavbarViewModel;
            createAccessViewModel.Query = menu;
            return createAccessViewModel;
        }
        public AccountAccessViewModel accountAccess()
        {
            BitArray check = new BitArray(1);
            check.Set(0, false);
            List<Role> role = _context.Roles.Where(i => i.IsDeleted == check).ToList();
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 6;
            AccountAccessViewModel accountAccessViewModel = new AccountAccessViewModel();
            accountAccessViewModel.Query = role;
            accountAccessViewModel.adminNavbarViewModel = adminNavbarViewModel;
            return accountAccessViewModel;
        }
        public PatientHistoryViewModel userAccess(string? firstname, string? lastname, string? email, string? phonenumber, int page = 1, int pageSize = 10)
        {
            IQueryable<User> users = _context.Users;
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 10;
            PatientHistoryViewModel patientHistoryViewModel = new PatientHistoryViewModel();
            patientHistoryViewModel.adminNavbarViewModel = adminNavbarViewModel;
            if (firstname != null)
            {
                users = users.Where(r => r.FirstName.ToLower().Contains(firstname.ToLower()));
            }
            if (lastname != null)
            {
                users = users.Where(r => r.LastName.ToLower().Contains(lastname.ToLower()));
            }
            if (email != null)
            {
                users = users.Where(r => r.Email.ToLower().Contains(email.ToLower()));
            }
            if (phonenumber != null)
            {
                users = users.Where(r => r.Mobile.Contains(phonenumber));
            }
            patientHistoryViewModel.CurrentPage = page;
            patientHistoryViewModel.PageSize = pageSize;
            patientHistoryViewModel.TotalItems = users.Count();
            patientHistoryViewModel.TotalPages = (int)Math.Ceiling((double)users.Count() / pageSize);
            //adminDashboardViewModel.requests = query.ToList();

            patientHistoryViewModel.Users = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return patientHistoryViewModel;
        }
        public ProviderViewModel providerInfo(int? region, int page = 1, int pageSize = 10)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 4;
            BitArray check = new BitArray(1);
            check.Set(0, false);
            IQueryable<Physician> query = _context.Physicians.Where(i => i.IsDeleted == check);
            if (region != null && region != -1)
            {
                query = query.Where(i => i.RegionId == region);
            }
            List<PhysicianList> physicianLists = new List<PhysicianList>();
            List<Physician> fquery = query.ToList();
            for (int i = 0; i < fquery.Count; i++)
            {
                BitArray bitArray = new BitArray(1);
                bitArray.Set(0, true);
                int n = i;
                Role role = _context.Roles.FirstOrDefault(i => i.RoleId == fquery[n].RoleId);
                bool notification;
                PhysicianNotification physicianNotification = _context.PhysicianNotifications.FirstOrDefault(i => i.PhysicianId == fquery[n].PhysicianId);
                if (physicianNotification.IsNotificationStopped == bitArray)
                {
                    notification = true;
                }
                else
                {
                    notification = false;
                }
                physicianLists.Add(new PhysicianList
                {
                    ProviderName = fquery[n]?.FirstName + " " + fquery[n]?.LastName,
                    RoleName = role?.Name,
                    OnCallStatus = 1,
                    Status = (int)fquery[n]?.Status,
                    Notification = notification,
                    PhysicianId = fquery[n].PhysicianId,
                });
            }
            List<Region> region1 = _context.Regions.ToList();
            ProviderViewModel providerViewModel = new ProviderViewModel();
            providerViewModel.adminNavbarViewModel = adminNavbarViewModel;
            providerViewModel.CurrentPage = 1;
            providerViewModel.PageSize = pageSize;
            providerViewModel.TotalItems = physicianLists.Count;
            providerViewModel.TotalPages = (int)Math.Ceiling((double)physicianLists.Count / pageSize);
            providerViewModel.PhysicianList = physicianLists.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            providerViewModel.Regions = region1;
            return providerViewModel;
        }
        public bool updateStopNotification(int id, bool stopNotification)
        {
            Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == id);

            if (physician != null)
            {
                //physician.Notification = new BitArray(new[] { isActive });
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool createAdmin(CreateAdminViewModel model)
        {
            try
            {
                AspNetUser aspNetUser = new AspNetUser();
                aspNetUser.UserName = model.LastName + model.FirstName[0];
                aspNetUser.PasswordHash = model.Password;
                aspNetUser.Email = model.Email;
                aspNetUser.PhoneNumber = model.PhoneNumber1;
                aspNetUser.CreatedDate = DateTime.Now;
                _context.AspNetUsers.Add(aspNetUser);
                _context.SaveChanges();

                var request = _httpContextAccessor.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwtService.getDetails(token);
                HalloDoc.DataLayer.Models.Admin admin = new HalloDoc.DataLayer.Models.Admin();

                admin.AspNetUserId = aspNetUser.Id;
                admin.FirstName = model.FirstName;
                admin.LastName = model.LastName;
                admin.Email = model.Email;
                admin.Mobile = model.PhoneNumber1;
                admin.Address1 = model.Address1;
                admin.Address2 = model.Address2;
                admin.City = model.city;
                admin.RegionId = model.StateId;
                admin.Zip = model.zip;
                admin.AltPhone = model.PhoneNumber2;
                admin.CreatedDate = DateTime.Now;
                admin.CreatedBy = cookieModel.aspId;
                admin.IsDeleted = false;
                admin.RoleId = Convert.ToInt32(model.Role);
                admin.Status = 1;
                _context.Admins.Add(admin);
                _context.SaveChanges();

                foreach (var regionIdString in model.SelectedRegion.Split(","))
                {
                    int regionId = int.Parse(regionIdString);
                    AdminRegion adminRegion = new AdminRegion();
                    adminRegion.RegionId = regionId;
                    adminRegion.AdminId = admin.AdminId;
                    _context.AdminRegions.Add(adminRegion);
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public CreateAdminViewModel createAdmin()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 15;
            List<Region> allRegions = _context.Regions.ToList();
            List<Role> roles = _context.Roles.ToList();
            CreateAdminViewModel createAdminViewModel = new CreateAdminViewModel();
            createAdminViewModel.adminNavbarViewModel = adminNavbarViewModel;
            createAdminViewModel.regions = allRegions;
            createAdminViewModel.Roles = roles;
            return createAdminViewModel;
        }

        public SearchRecordsViewModel searchRecord(string? patientName, string? providerName, string? email, string? phonenumber, int? selectedOptionValue, int? selectRequestType, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            BitArray check = new BitArray(1);
            check.Set(0, false);
            IQueryable<Request> query = _context.Requests.Include(i => i.RequestClient).Include(i => i.Physician).Where(i => i.IsDeleted == check);
            if (selectedOptionValue != 0 && selectedOptionValue != null)
            {
                if (selectedOptionValue == 1)
                {
                    query = query.Where(i => i.Status == 1);
                }
                else if (selectedOptionValue == 2)
                {
                    query = query.Where(i => i.Status == 2);
                }
                else if (selectedOptionValue == 3)
                {
                    query = query.Where(i => i.Status == 3 || i.Status == 4);
                }
                else if (selectedOptionValue == 4)
                {
                    query = query.Where(i => i.Status == 5);
                }
                else if (selectedOptionValue == 5)
                {
                    query = query.Where(i => i.Status == 6 || i.Status == 7 || i.Status == 8);
                }
                else
                {
                    query = query.Where(i => i.Status == 9);
                }
            }
            if (patientName != null)
            {
                query = query.Where(i => i.RequestClient.FirstName.ToLower().Contains(patientName.ToLower()));
            }
            if (selectRequestType != 0 && selectRequestType != null)
            {
                query = query.Where(i => i.RequestTypeId == selectRequestType);
            }
            //if(fromDate != null  && toDate != null)
            //{
            //    query = query.Where(i => i.RequestStatusLogs.Any(rsl => rsl.Status == 2 && rsl.CreatedDate >= fromDate && rsl.CreatedDate <= toDate));
            //}
            if (fromDate != null && toDate == null)
            {
                query = query.Where(r => r.AcceptedDate.Value.Date >= fromDate.Value.Date);
            }
            if (fromDate == null && toDate != null)
            {
                query = query.Where(r => r.AcceptedDate.Value.Date <= toDate.Value.Date);
            }
            if (fromDate != null && toDate != null)
            {
                query = query.Where(r => r.AcceptedDate.Value.Date >= fromDate.Value.Date && r.AcceptedDate.Value.Date <= toDate.Value.Date);
            }
            if (providerName != null)
            {
                query = query.Where(i => i.Physician.FirstName.ToLower().Contains(providerName.ToLower()));
            }
            if (email != null)
            {
                query = query.Where(i => i.RequestClient.Email.ToLower().Contains(email.ToLower()));
            }
            if (phonenumber != null)
            {
                query = query.Where(i => i.RequestClient.PhoneNumber.Contains(phonenumber));
            }
            SearchRecordsViewModel searchRecordsViewModel = new SearchRecordsViewModel();
            searchRecordsViewModel.CurrentPage = page;
            searchRecordsViewModel.PageSize = pageSize;
            searchRecordsViewModel.TotalItems = query.Count();
            searchRecordsViewModel.TotalPages = (int)Math.Ceiling((double)query.Count() / pageSize);
            List<SearchRecordTable> searchRecordTable = new List<SearchRecordTable>();
            List<Request> requests = query.ToList();
            for (int i = 0; i < requests.Count; i++)
            {
                int num = i;
                int id = requests[i].RequestId;
                RequestStatusLog requestStatusLog = _context.RequestStatusLogs.FirstOrDefault(i => i.RequestId == id);
                bool isActiveValue = requestStatusLog?.TransToAdmin?.Get(0) ?? false;

                RequestStatusLog requestStatusLogDOS = _context.RequestStatusLogs.FirstOrDefault(i => i.RequestId == id && i.Status == 2);
                RequestStatusLog requestStatusLogCCD = _context.RequestStatusLogs.FirstOrDefault(i => i.RequestId == id && i.Status == 9);
                RequestStatusLog requestStatusLogPCN = _context.RequestStatusLogs.FirstOrDefault(i => i.RequestId == id && i.Status == 2 && isActiveValue == true);
                RequestNote requestNote = _context.RequestNotes.FirstOrDefault(i => i.RequestId == id);
                searchRecordTable.Add(new SearchRecordTable
                {
                    PatientName = requests[i].RequestClient?.FirstName + " " + requests[i].RequestClient?.LastName,
                    Requestor = requests[i].RequestTypeId,
                    DateOfService = requestStatusLogDOS?.CreatedDate,
                    CloseCaseDate = requestStatusLogCCD?.CreatedDate,
                    Email = requests[i].RequestClient.Email,
                    PhoneNumber = requests[i].RequestClient?.PhoneNumber,
                    Address = requests[i].RequestClient?.Address + " ," + requests[i].RequestClient?.City + " ," + requests[i].RequestClient?.State,
                    Zip = requests[i].RequestClient?.ZipCode,
                    RequestStatus = requests[i].Status,
                    Physician = requests[i].Physician?.FirstName + " " + requests[i].Physician?.LastName,
                    PhysicianNote = requestNote?.PhysicianNotes,
                    //CancelledByProviderNote = requestStatusLogPCN?.Notes,
                    AdminNote = requestNote?.AdminNotes,
                    PatientNote = requests[i].RequestClient?.Notes,
                    RequestId = requests[i].RequestId,
                });
            }
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 7;
            searchRecordsViewModel.adminNavbarViewModel = adminNavbarViewModel;
            searchRecordsViewModel.ExcelData = searchRecordTable.ToList();
            searchRecordsViewModel.Query = searchRecordTable.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return searchRecordsViewModel;
        }
        public MemoryStream downloadSearchRecordsExcel(SearchRecordsViewModel model)
        {
            SearchRecordsViewModel searchRecordsViewModel = searchRecord(model.PatientName, model.ProviderName, model.Email, model.PhoneNumber, model.RequestStatus, model.RequestType, model.FromDate, model.ToDate);
            List<SearchRecordTable> data = searchRecordsViewModel.ExcelData;
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Data");

            worksheet.Cell(1, 1).Value = "Patient Name";
            worksheet.Cell(1, 2).Value = "Requestor";
            worksheet.Cell(1, 3).Value = "Date of Service";
            worksheet.Cell(1, 4).Value = "Close Case Date";
            worksheet.Cell(1, 5).Value = "Email";
            worksheet.Cell(1, 6).Value = "Phone Number";
            worksheet.Cell(1, 7).Value = "Address";
            worksheet.Cell(1, 8).Value = "Zip";
            worksheet.Cell(1, 9).Value = "Request Status";
            worksheet.Cell(1, 10).Value = "Physician";
            worksheet.Cell(1, 11).Value = "Physician Note";
            worksheet.Cell(1, 12).Value = "Admin Note";
            worksheet.Cell(1, 13).Value = "Patient Note";

            int row = 2;
            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = item.PatientName;

                worksheet.Cell(row, 2).Value = Enum.GetName(typeof(Statuses), item.Requestor);
                worksheet.Cell(row, 3).Value = item?.DateOfService;
                worksheet.Cell(row, 4).Value = item?.CloseCaseDate;
                worksheet.Cell(row, 5).Value = item.Email;
                worksheet.Cell(row, 6).Value = item.PhoneNumber;
                worksheet.Cell(row, 7).Value = item?.Address;
                worksheet.Cell(row, 8).Value = item?.Zip;
                worksheet.Cell(row, 9).Value = Enum.GetName(typeof(Status), item.RequestStatus);
                worksheet.Cell(row, 10).Value = item?.PhysicianNote;
                worksheet.Cell(row, 11).Value = item?.AdminNote;
                worksheet.Cell(row, 12).Value = item?.PatientNote;

                row++;
            }
            worksheet.Columns().AdjustToContents();
            var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
        public SearchRecordsViewModel searchRecords(string? patientName, string? providerName, string? email, string? phonenumber, int? selectedOptionValue, int? selectRequestType, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            SearchRecordsViewModel searchRecordsViewModel = searchRecord(patientName, providerName, email, phonenumber, selectedOptionValue, selectRequestType, fromDate, toDate, page, pageSize);
            return searchRecordsViewModel;
        }
        public PatientHistoryViewModel patientHistory(string? firstname, string? lastname, string? email, string? phonenumber, int page = 1, int pageSize = 10)
        {
            IQueryable<User> users = _context.Users;
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 10;
            PatientHistoryViewModel patientHistoryViewModel = new PatientHistoryViewModel();
            patientHistoryViewModel.adminNavbarViewModel = adminNavbarViewModel;
            if (firstname != null)
            {
                users = users.Where(r => r.FirstName.ToLower().Contains(firstname.ToLower()));
            }
            if (lastname != null)
            {
                users = users.Where(r => r.LastName.ToLower().Contains(lastname.ToLower()));
            }
            if (email != null)
            {
                users = users.Where(r => r.Email.ToLower().Contains(email.ToLower()));
            }
            if (phonenumber != null)
            {
                users = users.Where(r => r.Mobile.Contains(phonenumber));
            }
            patientHistoryViewModel.CurrentPage = page;
            patientHistoryViewModel.PageSize = pageSize;
            patientHistoryViewModel.TotalItems = users.Count();
            patientHistoryViewModel.TotalPages = (int)Math.Ceiling((double)users.Count() / pageSize);
            //adminDashboardViewModel.requests = query.ToList();

            patientHistoryViewModel.Users = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return patientHistoryViewModel;
        }
        public BlockHistoryViewModel blockHistory(string? firstname, DateTime? date, string? email, string? phonenumber, int page = 1, int pageSize = 10)
        {
            //IQueryable<BlockRequest> query = _context.BlockRequests.Include(i => i.Request);
            IQueryable<BlockRequest> query = _context.BlockRequests;
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 11;
            BlockHistoryViewModel blockHistoryViewModel = new BlockHistoryViewModel();
            blockHistoryViewModel.adminNavbarViewModel = adminNavbarViewModel;
            if (firstname != null)
            {
                query = query.Where(i => i.FirstName.ToLower().Contains(firstname.ToLower()));
            }
            if (date != null)
            {
                query = query.Where(b => b.CreatedDate == date.Value.Date);
            }

            if (email != null)
            {
                query = query.Where(b => b.Email.ToLower().Contains(email.ToLower()));
            }

            if (phonenumber != null)
            {
                query = query.Where(b => b.PhoneNumber.Contains(phonenumber));
            }
            blockHistoryViewModel.CurrentPage = page;
            blockHistoryViewModel.PageSize = pageSize;
            blockHistoryViewModel.TotalItems = query.Count();
            blockHistoryViewModel.TotalPages = (int)Math.Ceiling((double)query.Count() / pageSize);
            List<BlockRequest> queries = query.ToList();
            List<BlockHistoryTable> blockHistoryTable = new List<BlockHistoryTable>();
            for (int i = 0; i < queries.Count; i++)
            {
                int num = i;
                BitArray check = new BitArray(1);
                check.Set(0, false);
                bool isActiveValue = queries[i].IsActive.Get(0);  // Assuming bit 0 represents IsActive
                int requestId = queries[num].RequestId;
                var requestClient = _context.Requests
                                           .Include(i => i.RequestClient)
                                           .FirstOrDefault(i => i.RequestId == requestId && i.IsDeleted == check);
                //var requestClient = _context.Requests.Include(i => i.RequestClient).FirstOrDefault(i => i.RequestId == queries[num].RequestId && i.IsDeleted == check);

                blockHistoryTable.Add(new BlockHistoryTable()
                {
                    FirstName = requestClient?.FirstName,
                    PhoneNumber = queries[i].PhoneNumber,
                    CreatedDate = queries[i].CreatedDate,
                    Reason = queries[i].Reason,
                    Email = queries[i].Email,
                    IsActive = isActiveValue,
                    BlockRequestId = queries[i].BlockRequestId,
                });
            }
            blockHistoryViewModel.Query = blockHistoryTable.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return blockHistoryViewModel;
        }
        public bool unBlock(int id)
        {
            BlockRequest blockRequest = _context.BlockRequests.FirstOrDefault(i => i.BlockRequestId == id);


            if (blockRequest != null)
            {
                RequestStatusLog requestStatusLog = _context.RequestStatusLogs.Where(i => i.RequestId == blockRequest.RequestId).OrderByDescending(i => i.CreatedDate).Skip(1).Take(1).FirstOrDefault();
                Request request = _context.Requests.FirstOrDefault(i => i.RequestId == blockRequest.RequestId);
                request.Status = requestStatusLog.Status;
                _context.Requests.Update(request);
                _context.SaveChanges();
                RequestStatusLog requestStatusLog1 = new RequestStatusLog();
                requestStatusLog1.Status = requestStatusLog.Status;
                requestStatusLog1.CreatedDate = DateTime.Now;
                requestStatusLog1.RequestId = blockRequest.RequestId;
                _context.RequestStatusLogs.Add(requestStatusLog1);
                _context.SaveChanges();
                _context.BlockRequests.Remove(blockRequest);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool updateIsActive(int id, bool isActive)
        {
            BlockRequest blockRequest = _context.BlockRequests.FirstOrDefault(i => i.BlockRequestId == id);

            if (blockRequest != null)
            {
                blockRequest.IsActive = new BitArray(new[] { isActive });
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public EmailLogViewModel emailLog(string? receiverName, DateTime? date, DateTime? date2, string? email, string? role, int page, int pageSize)
        {
            IQueryable<EmailLog> query = _context.EmailLogs;
            //IQueryable<BlockRequest> query = _context.BlockRequests.Include(i => i.Request);
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 8;
            EmailLogViewModel emailLogViewModel = new EmailLogViewModel();
            emailLogViewModel.adminNavbarViewModel = adminNavbarViewModel;
            List<AspNetRole> aspNetRoles = _context.AspNetRoles.ToList();
            if (date != null)
            {
                query = query.Where(b => b.CreateDate.Date == date.Value.Date);
            }
            if (date2 != null)
            {
                query = query.Where(b => b.SentDate.Value.Date == date.Value.Date);
            }
            if (email != null)
            {
                query = query.Where(b => b.EmailId.ToLower().Contains(email.ToLower()));
            }
            if (role != null && role != "-1")
            {
                query = query.Where(b => b.RoleId == Convert.ToInt32(role));
            }
            emailLogViewModel.CurrentPage = page;
            //emailLogViewModel.Role = aspNetRoles;
            emailLogViewModel.PageSize = pageSize;
            emailLogViewModel.TotalItems = query.Count();
            emailLogViewModel.TotalPages = (int)Math.Ceiling((double)query.Count() / pageSize);
            List<EmailLog> queries = query.ToList();
            List<EmailLogTable> emailLogTable = new List<EmailLogTable>();
            for (int i = 0; i < queries.Count; i++)
            {
                int num = i;
                var recipient = "";
                var confirmationNumber = "-";
                if (queries[i].RoleId == 2)
                {
                    if (queries[i].RequestId != null)
                    {
                        var request2 = _context.Requests.Include(i => i.RequestClient).FirstOrDefault(i => i.RequestId == queries[num].RequestId);
                        recipient = request2.RequestClient.FirstName + " " + request2.RequestClient.LastName;
                        confirmationNumber = request2.ConfirmationNumber;
                    }
                    else
                    {
                        AspNetUser aspNetUser = _context.AspNetUsers.FirstOrDefault(i => i.Email == queries[num].EmailId);
                        User user = _context.Users.FirstOrDefault(i => i.AspNetUserId == aspNetUser.Id);
                        recipient = user.FirstName + " " + user.LastName;
                    }
                }
                else if (queries[i].RoleId == 1)
                {
                    if (queries[i].PhysicianId != null)
                    {
                        Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == queries[num].PhysicianId);
                        recipient = physician.FirstName + " " + physician.LastName;
                    }
                    else
                    {
                        AspNetUser aspNetUser = _context.AspNetUsers.FirstOrDefault(i => i.Email == queries[num].EmailId);
                        Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == queries[num].PhysicianId);
                        recipient = physician.FirstName + " " + physician.LastName;
                    }
                }
                else
                {
                    if (queries[i].AdminId != null)
                    {
                        HalloDoc.DataLayer.Models.Admin admin = _context.Admins.FirstOrDefault(i => i.AdminId == queries[num].AdminId);
                        recipient = admin.FirstName + " " + admin.LastName;
                    }
                    else
                    {
                        AspNetUser aspNetUser = _context.AspNetUsers.FirstOrDefault(i => i.Email == queries[num].EmailId);
                        HalloDoc.DataLayer.Models.Admin admin = _context.Admins.FirstOrDefault(i => i.AdminId == queries[num].AdminId);
                        recipient = admin.FirstName + " " + admin.LastName;
                    }
                }
                BitArray check = new BitArray(1);
                check.Set(0, false);

                emailLogTable.Add(new EmailLogTable()
                {
                    Recipient = recipient,
                    SentDate = queries[i].SentDate.Value,
                    CreatedDate = queries[i].CreateDate,
                    Action = "Request Monthly Data",
                    RoleName = (int)queries[i].RoleId,
                    Email = queries[i].EmailId,
                    Sent = queries[i].IsEmailSent == check ? "Yes" : "No",
                    SentTries = (int)queries[i].SentTries,
                    EmailLogId = queries[i].EmailLogId,
                    ConfirmationNumber = confirmationNumber,
                });
            }
            if (receiverName != null)
            {
                for (int i = 0; i < emailLogTable.Count(); i++)
                {
                    BitArray check = new BitArray(1);
                    check.Set(0, false);
                    emailLogTable = emailLogTable.Where(i => i.Recipient.ToLower().Contains(receiverName.ToLower())).Select(i => new EmailLogTable
                    {
                        Recipient = i.Recipient,
                        SentDate = i.SentDate,
                        CreatedDate = i.CreatedDate,
                        Action = "Request Monthly Data",
                        RoleName = i.RoleName,
                        Email = i.Email,
                        Sent = i.Sent,
                        SentTries = (int)i.SentTries,
                        EmailLogId = i.EmailLogId,
                        ConfirmationNumber = i.ConfirmationNumber,
                    }).ToList();
                }
            }

            emailLogViewModel.Query = emailLogTable.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return emailLogViewModel;
        }
        public bool deleteVendor(int id)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            HealthProfessional healthProfessional = _context.HealthProfessionals.FirstOrDefault(i => i.VendorId == id);
            if (healthProfessional != null)
            {
                BitArray check = new BitArray(1);
                check.Set(0, true);
                healthProfessional.IsDeleted = check;
                healthProfessional.ModifiedDate = DateTime.Now;
                _context.HealthProfessionals.Update(healthProfessional);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public VendorViewModel vendorInformation(string? vendorName, int? professionType, int page, int pageSize)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 5;
            BitArray check = new BitArray(1);
            check.Set(0, false);
            IQueryable<HealthProfessional> query = _context.HealthProfessionals.Where(i => i.IsDeleted == check);
            if (vendorName != null)
            {
                query = query.Where(i => i.VendorName.ToLower().Contains(vendorName.ToLower()));
            }
            if (professionType != null && professionType != -1)
            {
                query = query.Where(i => i.Profession == professionType);
            }
            List<VendorTable> vendorTables = new List<VendorTable>();
            List<HealthProfessional> fquery = query.ToList();
            for (int i = 0; i < fquery.Count; i++)
            {
                int n = i;
                HealthProfessionalType healthProfessionalType = _context.HealthProfessionalTypes.FirstOrDefault(i => i.HealthProfessionalId == fquery[n].Profession);
                vendorTables.Add(new VendorTable
                {
                    BusinessContact = fquery[n].BusinessContact,
                    BusinessName = fquery[n].VendorName,
                    Email = fquery[n].Email,
                    FaxNumber = fquery[n].FaxNumber,
                    PhoneNumber = fquery[n].PhoneNumber,
                    ProfessionName = healthProfessionalType.ProfessionName,
                    VendorId = fquery[n].VendorId,
                });
            }
            List<HealthProfessionalType> healthProfessionalTypes = _context.HealthProfessionalTypes.ToList();
            VendorViewModel vendorViewModel = new VendorViewModel();
            vendorViewModel.adminNavbarViewModel = adminNavbarViewModel;
            vendorViewModel.CurrentPage = 1;
            vendorViewModel.PageSize = pageSize;
            vendorViewModel.TotalItems = vendorTables.Count;
            vendorViewModel.TotalPages = (int)Math.Ceiling((double)vendorTables.Count / pageSize);
            vendorViewModel.Query = vendorTables.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            vendorViewModel.HealthProfessionalTypes = healthProfessionalTypes;
            return vendorViewModel;

        }
        public ProviderShift scheduling()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = cookieModel.name;
            adminNavbarViewModel.Tab = 13;
            List<Region> regions = _context.Regions.ToList();
            List<Physician> physicians = _context.Physicians.ToList();
            ProviderShift providerShift = new ProviderShift();
            providerShift.adminNavbarViewModel = adminNavbarViewModel;
            providerShift.RegionList = regions;
            providerShift.Physicians = physicians;
            return providerShift;
        }
        public bool createShift(ProviderShift model)
        {
            //CreateNewShift modal
            //ProviderShift model
            try
            {
                var request = _httpContextAccessor.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwtService.getDetails(token);

                BitArray IsRepeat = new BitArray(1);
                if (model.IsRepeat)
                {
                    IsRepeat.Set(0, true);
                }
                else
                {
                    IsRepeat.Set(0, false);
                }

                DateOnly startDate = new DateOnly(model.ShiftDate.Year, model.ShiftDate.Month, model.ShiftDate.Day);
                //startDate.Day = model.ShiftDate.Day;
                Shift shift = new Shift();
                shift.PhysicianId = model.PhysicianId;
                shift.CreatedBy = cookieModel.aspId;
                shift.CreatedDate = DateTime.Now;
                shift.StartDate = startDate;
                shift.IsRepeat = IsRepeat;
                shift.WeekDays = model.RepeatDays;
                shift.RepeatUpto = model.RepeatEnd;
                _context.Shifts.Add(shift);
                _context.SaveChanges();

                ShiftDetail shiftDetail = new ShiftDetail();
                shiftDetail.Shift = shift; 
                shiftDetail.ShiftDate = DateTime.Now;
                shiftDetail.RegionId = model.RegionId;
                shiftDetail.StartTime = model.StartTime;
                shiftDetail.EndTime = model.EndTime;
                shiftDetail.Status = 0;
                shiftDetail.IsDeleted = new BitArray(1, false);
                _context.ShiftDetails.Add(shiftDetail);
                _context.SaveChanges();

                string ShiftStartDay = model.ShiftDate.DayOfWeek.ToString();
                int intDay = dayOfWeek(ShiftStartDay);
                if (model.IsRepeat)
                {
                    string repeatDays = model.RepeatDays;
                    for (int i = 0; i < repeatDays.Length; i++)
                    {
                        if (repeatDays[i].Equals('1'))
                        {
                            int diff;
                            if (intDay > i)
                            {
                                diff = 7 - Math.Abs(i - intDay);
                            }
                            else
                            {
                                diff = Math.Abs(i - intDay);
                            }
                            DateTime temp = model.ShiftDate.AddDays(diff);
                            ShiftDetail shiftDetail1 = new ShiftDetail();
                            shiftDetail1.ShiftId = shift.ShiftId;
                            shiftDetail1.ShiftDate = temp;
                            shiftDetail1.RegionId = model.RegionId;
                            shiftDetail1.StartTime = model.StartTime;
                            shiftDetail1.EndTime = model.EndTime;
                            shiftDetail1.Status = 0;
                            shiftDetail1.IsDeleted = new BitArray(1, false);
                            _context.ShiftDetails.Add(shiftDetail1);
                            _context.SaveChanges();

                            for (int j = 0; j < model.RepeatEnd - 1; j++)
                            {
                                ShiftDetail shiftDetails = new ShiftDetail();
                                shiftDetails.ShiftId = shift.ShiftId;
                                shiftDetails.ShiftDate = temp.AddDays(7);
                                shiftDetails.RegionId = model.RegionId;
                                shiftDetails.StartTime = model.StartTime;
                                shiftDetails.EndTime = model.EndTime;
                                shiftDetails.Status = 0;
                                shiftDetails.IsDeleted = new BitArray(1, false);
                                _context.ShiftDetails.Add(shiftDetails);
                                _context.SaveChanges();
                                temp = temp.AddDays(7);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

            //try
            //{
            //    string WeekDays = "";
            //    if (modal.RepeatDays != null)
            //    {
            //        foreach (var item in modal.RepeatDays)
            //        {
            //            WeekDays += item + ",";
            //        }
            //        WeekDays = WeekDays.Substring(0, WeekDays.Length - 1);
            //    }

            //    Shift shift = new Shift()
            //    {
            //        PhysicianId = modal.PhysicianId,
            //        StartDate = DateOnly.Parse(modal.ShiftDate),
            //        IsRepeat = new System.Collections.BitArray(new[] { modal.IsRepeat }),
            //        WeekDays = WeekDays,
            //        RepeatUpto = modal.RepeatUpto,
            //        CreatedBy = cookieModel.aspId,
            //    };
            //    _context.Shifts.Add(shift);
            //    _context.SaveChanges();

            //    if (modal.RepeatDays == null)
            //    {
            //        modal.RepeatDays = new List<int>();
            //    }

            //    DateTime ShiftDate = DateTime.Parse(modal.ShiftDate.ToString());

            //    DateTime NexttDate = ShiftDate;
            //    int j = 0;

            //    for (int i = 0; i <= modal.RepeatUpto * modal.RepeatDays.Count(); i++)
            //    {
            //        ShiftDetail shiftDetail = new ShiftDetail()
            //        {
            //            ShiftId = shift.ShiftId,
            //            ShiftDate = NexttDate,
            //            RegionId = modal.PhysicianRegion,
            //            StartTime = modal.StartTime,
            //            EndTime = modal.EndTime,
            //            Status = 1,
            //            IsDeleted = new System.Collections.BitArray(new[] { false })
            //        };
            //        _context.ShiftDetails.Add(shiftDetail);

            //        if (modal.RepeatDays.Count() > 0)
            //        {
            //            int skipDay = (7 - (int)ShiftDate.DayOfWeek - 1 + modal.RepeatDays[j]);
            //            if (skipDay > 7)
            //            {
            //                skipDay = skipDay % 7;
            //            }

            //            NexttDate = ShiftDate.AddDays(skipDay);
            //            ShiftDate = NexttDate;

            //            if (i % modal.RepeatUpto == modal.RepeatUpto - 1 && i != modal.RepeatUpto * modal.RepeatDays.Count() - 1)
            //            {
            //                j++;
            //                ShiftDate = DateTime.Parse(modal.ShiftDate.ToString());
            //            }

            //        }
            //        _context.SaveChanges();
            //    }
            //    return true;
            //}
            //catch { return false; }
        }
        public ShiftDetail viewShift(int id)
        {
            var shiftDetail = _context.ShiftDetails.Include(i => i.Shift).FirstOrDefault(i => i.ShiftDetailId == id);
            return shiftDetail;
        }
        public int dayOfWeek(string day)
        {
            if (day == "Monday")
            {
                return 0;
            }
            else if (day == "Tuesday")
            {
                return 1;
            }
            else if (day == "Wednesday")
            {
                return 2;
            }
            else if (day == "Thursday")
            {
                return 3;
            }
            else if (day == "Friday")
            {
                return 4;
            }
            else if (day == "Saturday")
            {
                return 5;
            }
            else
            {
                return 6;
            }
        }
        public List<ProviderInformationViewModel> GetProviderInformation(int Region)
        {
            try
            {
                var physician = _context.PhysicianRegions.Include(m => m.Physician).Where(m => Region == 0 || m.RegionId == Region);
                List<ProviderInformationViewModel> model = new List<ProviderInformationViewModel>();
                foreach (var item in physician)
                {
                    if (item.Physician.IsDeleted == null || item.Physician.IsDeleted[0] == false)
                    {
                        ProviderInformationViewModel providerInformationViewModel = new ProviderInformationViewModel()
                        {
                            PhysicianId = item.Physician.PhysicianId,
                            ProviderName = item.Physician.FirstName + " " + item.Physician.LastName,
                            ProviderEmail = item.Physician.Email,
                            Role = item.Physician.RoleId.ToString(),
                            Status = item.Physician.Status.ToString()
                        };
                        model.Add(providerInformationViewModel);
                    }
                }
                return model;
            }
            catch
            {
                return new List<ProviderInformationViewModel>();
            }
        }
        public List<ShiftDetail> GetScheduleData(int RegionId)
        {
            try
            {
                return _context.ShiftDetails.Include(m => m.Shift).Where(m => (m.RegionId == RegionId || RegionId == null || RegionId == 0) && m.IsDeleted == new System.Collections.BitArray(new[] { false })).ToList();
            }
            catch { return new List<ShiftDetail> { }; }
        }
        public string GetPhyFromId(int id, int shiftId)
        {
            Physician p = _context.Physicians.Where(ph => ph.PhysicianId == id).FirstOrDefault();
            ShiftDetail shiftDetail = _context.ShiftDetails.FirstOrDefault(i => i.ShiftId == shiftId);
            Region region = _context.Regions.FirstOrDefault(i => i.RegionId == shiftDetail.RegionId);
            return "/ " + p.LastName.ToUpper() + " " + p.FirstName[0] + ". " + region.Abbreviation;
        }
        public bool deleteShift(int id)
        {
            try
            {
                ShiftDetail shiftDetail = _context.ShiftDetails.FirstOrDefault(i => i.ShiftDetailId == id);
                shiftDetail.IsDeleted = new BitArray(1, true);
                _context.ShiftDetails.Update(shiftDetail);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool editShift(ProviderShift model)
        {
            try
            {
                ShiftDetail shiftDetail = _context.ShiftDetails.FirstOrDefault(i => i.ShiftDetailId == model.ShiftDetailId);
                shiftDetail.ShiftDate = model.ShiftDate;
                shiftDetail.StartTime = model.StartTime;
                shiftDetail.EndTime = model.EndTime;
                _context.ShiftDetails.Update(shiftDetail);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public ShiftReviewViewModel filterShiftDetail(int? region, int page = 1, int pageSize = 10)
        {
            //var query = from sd in _context.ShiftDetails
            //            join r in _context.Regions on sd.RegionId equals r.RegionId // Join with Regions table
            //            join s in _context.Shifts on sd.ShiftId equals s.ShiftId
            //            join p in _context.Physicians on s.PhysicianId equals p.PhysicianId
            //            select new ShiftDetailData
            //            {
            //                ShiftDetailId = sd.ShiftDetailId,
            //                PhysicianName = p.FirstName + " " + p.LastName,
            //                ShiftDate = sd.ShiftDate.ToString("MMM dd, yyyy"),
            //                Region = r.Name,
            //                StartTime = sd.StartTime,
            //                EndTime = sd.EndTime,
            //            };

            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 13;
            BitArray check = new BitArray(1);
            check.Set(0, false);
            IQueryable<ShiftDetail> query = _context.ShiftDetails.Include(i => i.Shift).Where(i => i.IsDeleted == check);
            if (region != null && region != -1)
            {
                query = query.Where(i => i.RegionId == region);
            }
            List<ShiftDetailData> shiftDetailList = new List<ShiftDetailData>();
            List<ShiftDetail> fquery = query.ToList();
            for (int i = 0; i < fquery.Count; i++)
            {
                BitArray bitArray = new BitArray(1);
                bitArray.Set(0, true);
                int n = i;
                //Role role = _context.Roles.FirstOrDefault(i => i.RoleId == fquery[n].RoleId);
                bool notification;
                Region region2 = _context.Regions.FirstOrDefault(i => i.RegionId == fquery[n].RegionId);
                Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == fquery[n].Shift.PhysicianId);
                shiftDetailList.Add(new ShiftDetailData
                {
                    PhysicianName = physician.FirstName + " " + physician.LastName,
                    ShiftDetailId = fquery[n].ShiftDetailId,
                    ShiftDate = fquery[n].ShiftDate.ToString("MMM dd, yyyy"),
                    StartTime = fquery[n].StartTime,
                    EndTime = fquery[n].EndTime,
                    Region = region2.Name,
                });
            }
            List<Region> region1 = _context.Regions.ToList();
            ShiftReviewViewModel shiftReviewViewModel = new ShiftReviewViewModel();
            shiftReviewViewModel.adminNavbarViewModel = adminNavbarViewModel;
            shiftReviewViewModel.CurrentPage = 1;
            shiftReviewViewModel.PageSize = pageSize;
            shiftReviewViewModel.TotalItems = shiftDetailList.Count;
            shiftReviewViewModel.TotalPages = (int)Math.Ceiling((double)shiftDetailList.Count / pageSize);
            shiftReviewViewModel.ShiftDetails = shiftDetailList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            shiftReviewViewModel.Regions = region1;
            return shiftReviewViewModel;
        }
        public bool returnShift(int id)
        {
            try
            {
                ShiftDetail shiftDetail = _context.ShiftDetails.FirstOrDefault(i => i.ShiftDetailId == id);
                shiftDetail.Status = shiftDetail.Status == 1 ? (short)0 : (short)1;
                _context.ShiftDetails.Update(shiftDetail);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool contactProvider(ProviderViewModel model)
        {
            Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == model.PhysicianId);
            if (physician != null)
            {
                BitArray check = new BitArray(1);
                check.Set(0, true);
                if (model.CommunicationType == 2 || model.CommunicationType == 3)
                {
                    string senderEmail = "tatva.dotnet.karmadipsinhsolanki@outlook.com";
                    string senderPassword = "Karmadips@2311";

                    SmtpClient client = new SmtpClient("smtp.office365.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(senderEmail, senderPassword),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false
                    };
                    string email = physician.Email;
                    string message = model.Message;
                    if (email != null)
                    {
                        MailMessage mailMessage = new MailMessage
                        {
                            From = new MailAddress(senderEmail, "HalloDoc"),
                            Subject = "Message from Admin",
                            IsBodyHtml = true,
                            Body = message,
                        };
                        mailMessage.To.Add(email);
                        client.Send(mailMessage);
                    }

                    EmailLog emailLog = new EmailLog();
                    emailLog.SubjectName = "Message from Admin";
                    emailLog.EmailId = physician.Email;
                    emailLog.CreateDate = DateTime.Now;
                    emailLog.SentDate = DateTime.Now;
                    emailLog.SentTries = 1;
                    emailLog.IsEmailSent = check;
                    emailLog.RoleId = 1;
                    emailLog.PhysicianId = physician.PhysicianId;
                    emailLog.EmailTemplate = model.Message;
                    _context.EmailLogs.Add(emailLog);
                    _context.SaveChanges();
                }
                if (model.CommunicationType == 1 || model.CommunicationType == 3)
                {
                    string messageSMS = model.Message;

                    var accountSid = _configuration["Twilio:accountSid"];
                    var authToken = _configuration["Twilio:authToken"];
                    var twilionumber = _configuration["Twilio:twilioNumber"];

                    TwilioClient.Init(accountSid, authToken);
                    //var messageBody =
                    var message2 = MessageResource.Create(
                        from: new Twilio.Types.PhoneNumber(twilionumber),
                        body: messageSMS,
                        to: new Twilio.Types.PhoneNumber(physician.Mobile)
                    );


                    var request2 = _httpContextAccessor.HttpContext.Request;
                    var token = request2.Cookies["jwt"];
                    CookieModel cookieModel = _jwtService.getDetails(token);

                    Smslog smslog = new Smslog();
                    smslog.IsSmssent = check;
                    smslog.MobileNumber = physician.Mobile;
                    smslog.Smstemplate = model.Message;
                    smslog.SentDate = DateTime.Now;
                    smslog.CreateDate = DateTime.Now;
                    smslog.SentTries = 1;
                    smslog.PhysicianId = physician.PhysicianId;
                    smslog.RoleId = 1;
                    _context.Smslogs.Add(smslog);
                    _context.SaveChanges();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool editPhysicianOnboarding(CreateProviderViewModel model)
        {
            try
            {

                BitArray IsICA = new BitArray(1);
                if (model.IsICA)
                {
                    IsICA.Set(0, true);
                }
                else
                {
                    IsICA.Set(0, false);
                }
                BitArray IsBackgroundCheck = new BitArray(1);
                if (model.IsBackgroundCheck)
                {
                    IsBackgroundCheck.Set(0, true);
                }
                else
                {
                    IsBackgroundCheck.Set(0, false);
                }
                BitArray IsHIPAACompliance = new BitArray(1);
                if (model.IsHIPAACompliance)
                {
                    IsHIPAACompliance.Set(0, true);
                }
                else
                {
                    IsHIPAACompliance.Set(0, false);
                }
                BitArray IsNonDisclosureAgreement = new BitArray(1);
                if (model.IsNonDisclosureAgreement)
                {
                    IsNonDisclosureAgreement.Set(0, true);
                }
                else
                {
                    IsNonDisclosureAgreement.Set(0, false);
                }
                string filepath = "physician/" + model.PhysicianId;
                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", filepath);
                if (model.ICA != null && model.ICA.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "ICA." + model.ICA.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.ICA.CopyToAsync(stream);
                    }
                }
                if (model.HIPAACompliance != null && model.HIPAACompliance.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "HIPAACompliance." + model.HIPAACompliance.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.HIPAACompliance.CopyToAsync(stream);
                    }
                }
                if (model.BackgroundCheck != null && model.BackgroundCheck.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "BackgroundCheck." + model.BackgroundCheck.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.BackgroundCheck.CopyToAsync(stream);
                    }
                }
                if (model.NonDisclosureAgreement != null && model.NonDisclosureAgreement.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "NonDisclosureAgreement." + model.NonDisclosureAgreement.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.NonDisclosureAgreement.CopyToAsync(stream);
                    }
                }
                var request = _httpContextAccessor.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwtService.getDetails(token);
                Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == model.PhysicianId);
                physician.IsAgreementDoc = IsICA;
                physician.IsBackgroundDoc = IsBackgroundCheck;
                physician.IsCredentialDoc = IsHIPAACompliance;
                physician.IsNonDisclosureDoc = IsNonDisclosureAgreement;
                physician.ModifiedDate = DateTime.Now;
                physician.ModifiedBy = cookieModel.aspId;
                _context.Physicians.Update(physician);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool editPhysicianProfile(CreateProviderViewModel model)
        {
            try
            {
                var request = _httpContextAccessor.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwtService.getDetails(token);
                Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == model.PhysicianId);
                physician.BusinessName = model.BusinessName;
                physician.BusinessWebsite = model.BusinessWebsite;
                physician.AdminNotes = model.AdminNotes;
                if (model.IsPhoto == "true")
                {
                    physician.Photo = model.Photo.FileName;
                }
                physician.ModifiedBy = cookieModel.aspId;
                physician.ModifiedDate = DateTime.Now;
                _context.Physicians.Update(physician);
                _context.SaveChanges();

                string filepath = "physician/" + physician.PhysicianId;
                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", filepath);
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "photo." + model.Photo.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.Photo.CopyToAsync(stream)
    ;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool editPhysicianMailingInformation(CreateProviderViewModel model)
        {
            try
            {
                Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == model.PhysicianId);
                physician.Address1 = model.Address1;
                physician.Address2 = model.Address2;
                physician.City = model.City;
                physician.RegionId = model.StateId;
                physician.AltPhone = model.AltPhoneNumber;
                _context.Physicians.Update(physician);
                _context.SaveChanges();

                PhysicianLocation physicianLocation = _context.PhysicianLocations.FirstOrDefault(i => i.PhysicianId == model.PhysicianId);
                physicianLocation.Longitude = model.Longitude;
                physicianLocation.Latitude = model.Latitude;
                physicianLocation.PhysicianId = physician.PhysicianId;
                physicianLocation.Address = physician.Address1 + "," + physician.Address2 + " ," + physician.City;
                _context.PhysicianLocations.Update(physicianLocation);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool editPhysicianInformation(CreateProviderViewModel model)
        {
            try
            {
                var request = _httpContextAccessor.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwtService.getDetails(token);
                Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == model.PhysicianId);
                physician.FirstName = model.FirstName;
                physician.LastName = model.LastName;
                physician.Email = model.Email;
                physician.Mobile = model.PhoneNumber;
                physician.MedicalLicense = model.MedicalLicense;
                physician.Npinumber = model.NPINumber;
                _context.Physicians.Update(physician);
                _context.SaveChanges();

                AspNetUser aspNetUser = _context.AspNetUsers.FirstOrDefault(i => i.Id == physician.AspNetUserId);
                aspNetUser.Email = model.Email;
                aspNetUser.PhoneNumber = model.PhoneNumber;
                _context.AspNetUsers.Update(aspNetUser);
                _context.SaveChanges();

                foreach (var physicianRegion in _context.PhysicianRegions.Where(ar => ar.PhysicianId == model.PhysicianId))
                {
                    _context.PhysicianRegions.Remove(physicianRegion);
                }
                _context.SaveChanges();

                string selectedRegionString = model.SelectedRegion;

                if (!string.IsNullOrEmpty(selectedRegionString))
                {
                    string[] regionIds = selectedRegionString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var regionIdString in regionIds)
                    {
                        int regionId = int.Parse(regionIdString);
                        PhysicianRegion physicianRegion = new PhysicianRegion();
                        physicianRegion.RegionId = regionId;
                        physicianRegion.PhysicianId = physician.PhysicianId;
                        _context.PhysicianRegions.Add(physicianRegion);
                    }
                }
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool deletePhysicianAccount(int id)
        {
            try
            {
                BitArray IsDeleted = new BitArray(1);
                IsDeleted.Set(0, true);
                Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == id);
                physician.IsDeleted = IsDeleted;
                _context.Physicians.Update(physician);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool editPhysicianPassword(string password, int id)
        {
            try
            {
                Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == id);
                AspNetUser aspNetUser = _context.AspNetUsers.FirstOrDefault(i => i.Id == physician.AspNetUserId);
                aspNetUser.PasswordHash = password;
                _context.AspNetUsers.Update(aspNetUser);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool editPhysicianAccountInformation(CreateProviderViewModel model)
        {
            try
            {

                var request = _httpContextAccessor.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwtService.getDetails(token);
                Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == model.PhysicianId);
                physician.RoleId = Convert.ToInt32(model.Role);
                physician.ModifiedDate = DateTime.Now;
                physician.ModifiedBy = cookieModel.aspId;
                physician.Status = (short)model.Status;
                _context.Physicians.Update(physician);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool editPhysicianAccount(CreateProviderViewModel model)
        {
            try
            {
                BitArray IsICA = new BitArray(1);
                if (model.IsICA)
                {
                    IsICA.Set(0, true);
                }
                else
                {
                    IsICA.Set(0, false);
                }
                BitArray IsBackgroundCheck = new BitArray(1);
                if (model.IsBackgroundCheck)
                {
                    IsBackgroundCheck.Set(0, true);
                }
                else
                {
                    IsBackgroundCheck.Set(0, false);
                }
                BitArray IsHIPAACompliance = new BitArray(1);
                if (model.IsHIPAACompliance)
                {
                    IsHIPAACompliance.Set(0, true);
                }
                else
                {
                    IsHIPAACompliance.Set(0, false);
                }
                BitArray IsNonDisclosureAgreement = new BitArray(1);
                if (model.IsNonDisclosureAgreement)
                {
                    IsNonDisclosureAgreement.Set(0, true);
                }
                else
                {
                    IsNonDisclosureAgreement.Set(0, false);
                }
                BitArray IsDeleted = new BitArray(1);
                IsDeleted.Set(0, false);
                Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == model.PhysicianId && i.IsDeleted == IsDeleted);
                //physician.Photo = model.Photo.FileName;
                //physician.FirstName = model.FirstName;
                var request = _httpContextAccessor.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwtService.getDetails(token);
                //physician.AspNetUserId = aspNetUser.Id;
                physician.FirstName = model.FirstName;
                physician.LastName = model.LastName;
                physician.Email = model.Email;
                physician.Mobile = model.PhoneNumber;
                physician.MedicalLicense = model.MedicalLicense;
                physician.Photo = model.Photo.FileName;
                physician.AdminNotes = model.AdminNotes;
                physician.IsAgreementDoc = IsICA;
                physician.IsBackgroundDoc = IsBackgroundCheck;
                physician.IsCredentialDoc = IsHIPAACompliance;
                physician.IsNonDisclosureDoc = IsNonDisclosureAgreement;
                physician.Address1 = model.Address1;
                physician.Address2 = model.Address2;
                physician.City = model.City;
                physician.RegionId = model.StateId;
                physician.Zip = model.Zip;
                physician.AltPhone = model.AltPhoneNumber;
                physician.CreatedBy = cookieModel.aspId;
                physician.CreatedDate = DateTime.Now;
                physician.RoleId = Convert.ToInt32(model.Role);
                physician.BusinessName = model.BusinessName;
                physician.BusinessWebsite = model.BusinessWebsite;
                physician.IsDeleted = IsDeleted;
                physician.Npinumber = model.NPINumber;
                physician.Status = 1;
                _context.Physicians.Update(physician);
                _context.SaveChanges();

                PhysicianLocation physicianLocation = _context.PhysicianLocations.FirstOrDefault(i => i.PhysicianId == model.PhysicianId);
                physicianLocation.Longitude = model.Longitude;
                physicianLocation.Latitude = model.Latitude;
                physicianLocation.PhysicianName = model.FirstName + " " + model.LastName;
                physicianLocation.PhysicianId = physician.PhysicianId;
                physicianLocation.Address = physician.Address1 + "," + physician.Address2 + " ," + physician.City;
                physicianLocation.CreatedDate = DateTime.Now;
                _context.PhysicianLocations.Update(physicianLocation);
                _context.SaveChanges();

                foreach (var physicianRegion in _context.PhysicianRegions.Where(ar => ar.PhysicianId == model.PhysicianId))
                {
                    _context.PhysicianRegions.Remove(physicianRegion);
                }
                _context.SaveChanges();

                string selectedRegionString = model.SelectedRegion;

                if (!string.IsNullOrEmpty(selectedRegionString))
                {
                    string[] regionIds = selectedRegionString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var regionIdString in regionIds)
                    {
                        int regionId = int.Parse(regionIdString);
                        PhysicianRegion physicianRegion = new PhysicianRegion();
                        physicianRegion.RegionId = regionId;
                        physicianRegion.PhysicianId = physician.PhysicianId;
                        _context.PhysicianRegions.Add(physicianRegion);
                    }
                }
                _context.SaveChanges();

                string filepath = "physician/" + physician.PhysicianId;
                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", filepath);
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "photo." + model.Photo.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.Photo.CopyToAsync(stream)
;
                    }
                }
                if (model.ICA != null && model.ICA.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "ICA." + model.ICA.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.ICA.CopyToAsync(stream);
                    }
                }
                if (model.HIPAACompliance != null && model.HIPAACompliance.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "HIPAACompliance." + model.HIPAACompliance.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.HIPAACompliance.CopyToAsync(stream);
                    }
                }
                if (model.BackgroundCheck != null && model.BackgroundCheck.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "BackgroundCheck." + model.BackgroundCheck.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.BackgroundCheck.CopyToAsync(stream);
                    }
                }
                if (model.NonDisclosureAgreement != null && model.NonDisclosureAgreement.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "NonDisclosureAgreement." + model.NonDisclosureAgreement.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.NonDisclosureAgreement.CopyToAsync(stream);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public CreateProviderViewModel editPhysicianAccount(int id)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 4;
            List<Region> regions = _context.Regions.ToList();
            List<Role> roles = _context.Roles.ToList();
            CreateProviderViewModel createProviderViewModel = new CreateProviderViewModel();
            createProviderViewModel.adminNavbarViewModel = adminNavbarViewModel;
            //var PhysicianDetails = _context.Admins.FirstOrDefault(u => u.AspNetUserId == aspNetUserId);

            createProviderViewModel.Regions = regions;
            createProviderViewModel.Roles = roles;

            BitArray check = new BitArray(1);
            check.Set(0, true);
            BitArray IsDeleted = new BitArray(1);
            IsDeleted.Set(0, false);
            Physician physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == id && i.IsDeleted == IsDeleted);
            AspNetUser aspNetUser = _context.AspNetUsers.FirstOrDefault(i => i.Id == physician.AspNetUserId);
            createProviderViewModel.FirstName = physician.FirstName;
            createProviderViewModel.LastName = physician.LastName;
            createProviderViewModel.Role = physician.RoleId.ToString();
            createProviderViewModel.Email = physician.Email;
            createProviderViewModel.PhoneNumber = physician.Mobile;
            createProviderViewModel.MedicalLicense = physician.MedicalLicense;
            createProviderViewModel.NPINumber = physician.Npinumber;
            createProviderViewModel.Address1 = physician.Address1;
            createProviderViewModel.Address2 = physician.Address2;
            createProviderViewModel.City = physician.City;
            createProviderViewModel.StateId = (int)physician.RegionId;
            createProviderViewModel.Zip = physician.Zip;
            createProviderViewModel.AltPhoneNumber = physician.AltPhone;
            createProviderViewModel.BusinessName = physician.BusinessName;
            createProviderViewModel.BusinessWebsite = physician.BusinessWebsite;
            createProviderViewModel.AdminNotes = physician.AdminNotes;
            createProviderViewModel.IsBackgroundCheck = physician.IsBackgroundDoc[0] == true ? true : false;
            createProviderViewModel.IsICA = physician.IsAgreementDoc[0] == true ? true : false;
            createProviderViewModel.IsHIPAACompliance = physician.IsCredentialDoc[0] == true ? true : false;
            createProviderViewModel.IsNonDisclosureAgreement = physician.IsNonDisclosureDoc[0] == true ? true : false;
            createProviderViewModel.PhotoName = physician.Photo == null ? "Select File" : physician.Photo;
            createProviderViewModel.check = true;
            createProviderViewModel.PhysicianId = physician.PhysicianId;
            createProviderViewModel.Username = aspNetUser.UserName;
            createProviderViewModel.Status = (short)physician.Status;
            PhysicianLocation physicianLocation = _context.PhysicianLocations.FirstOrDefault(i => i.PhysicianId == id);
            createProviderViewModel.Latitude = (decimal)physicianLocation.Latitude;
            createProviderViewModel.Longitude = (decimal)physicianLocation.Longitude;
            var allRegions = _context.Regions.ToList();

            List<ProviderSelectedRegions> selectedRegions = allRegions.Select(r => new ProviderSelectedRegions
            {
                IsSelected = _context.PhysicianRegions.Any(ar => ar.PhysicianId == physician.PhysicianId && ar.RegionId == r.RegionId),
                Name = r.Name,
                RegionId = r.RegionId
            })
            .ToList();
            createProviderViewModel.State = selectedRegions;
            return createProviderViewModel;

        }
        public bool createPhysician(CreateProviderViewModel model)
        {
            try
            {
                var request = _httpContextAccessor.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwtService.getDetails(token);
                AspNetUser aspNetUser = new AspNetUser();
                aspNetUser.Email = model.Email;
                aspNetUser.PasswordHash = model.Password;
                aspNetUser.UserName = "MD." + model.LastName + model.FirstName[0];
                aspNetUser.PhoneNumber = model.PhoneNumber;
                aspNetUser.CreatedDate = DateTime.Now;
                _context.AspNetUsers.Add(aspNetUser);
                _context.SaveChanges();

                BitArray IsICA = new BitArray(1);
                if (model.IsICA)
                {
                    IsICA.Set(0, true);
                }
                else
                {
                    IsICA.Set(0, false);
                }
                BitArray IsBackgroundCheck = new BitArray(1);
                if (model.IsBackgroundCheck)
                {
                    IsBackgroundCheck.Set(0, true);
                }
                else
                {
                    IsBackgroundCheck.Set(0, false);
                }
                BitArray IsHIPAACompliance = new BitArray(1);
                if (model.IsHIPAACompliance)
                {
                    IsHIPAACompliance.Set(0, true);
                }
                else
                {
                    IsHIPAACompliance.Set(0, false);
                }
                BitArray IsNonDisclosureAgreement = new BitArray(1);
                if (model.IsNonDisclosureAgreement)
                {
                    IsNonDisclosureAgreement.Set(0, true);
                }
                else
                {
                    IsNonDisclosureAgreement.Set(0, false);
                }
                BitArray IsDeleted = new BitArray(1);
                IsDeleted.Set(0, false);
                Physician physician = new Physician();
                physician.AspNetUserId = aspNetUser.Id;
                physician.FirstName = model.FirstName;
                physician.LastName = model.LastName;
                physician.Email = model.Email;
                physician.Mobile = model.PhoneNumber;
                physician.MedicalLicense = model.MedicalLicense;
                physician.Photo = model.Photo.FileName;
                physician.AdminNotes = model.AdminNotes;
                physician.IsAgreementDoc = IsICA;
                physician.IsBackgroundDoc = IsBackgroundCheck;
                physician.IsCredentialDoc = IsHIPAACompliance;
                physician.IsNonDisclosureDoc = IsNonDisclosureAgreement;
                physician.Address1 = model.Address1;
                physician.Address2 = model.Address2;
                physician.City = model.City;
                physician.RegionId = model.StateId;
                physician.Zip = model.Zip;
                physician.AltPhone = model.AltPhoneNumber;
                physician.CreatedBy = cookieModel.aspId;
                physician.CreatedDate = DateTime.Now;
                physician.RoleId = Convert.ToInt32(model.Role);
                physician.BusinessName = model.BusinessName;
                physician.BusinessWebsite = model.BusinessWebsite;
                physician.IsDeleted = IsDeleted;
                physician.Npinumber = model.NPINumber;
                physician.Status = 1;
                _context.Physicians.Add(physician);
                _context.SaveChanges();

                PhysicianLocation physicianLocation = new PhysicianLocation();
                physicianLocation.Longitude = model.Longitude;
                physicianLocation.Latitude = model.Latitude;
                physicianLocation.PhysicianName = model.FirstName + " " + model.LastName;
                physicianLocation.PhysicianId = physician.PhysicianId;
                physicianLocation.Address = physician.Address1 + "," + physician.Address2 + " ," + physician.City;
                physicianLocation.CreatedDate = DateTime.Now;
                _context.PhysicianLocations.Add(physicianLocation);
                _context.SaveChanges();

                BitArray IsNotificationStopped = new BitArray(1);
                IsNotificationStopped.Set(0, true);

                PhysicianNotification physicianNotification = new PhysicianNotification();
                physicianNotification.PhysicianId = physician.PhysicianId;
                physicianNotification.IsNotificationStopped = IsNotificationStopped;
                _context.PhysicianNotifications.Add(physicianNotification);
                _context.SaveChanges();

                string selectedRegionString = model.SelectedRegion;

                if (!string.IsNullOrEmpty(selectedRegionString))
                {
                    string[] regionIds = selectedRegionString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var regionIdString in regionIds)
                    {
                        int regionId = int.Parse(regionIdString);
                        PhysicianRegion physicianRegion = new PhysicianRegion();
                        physicianRegion.RegionId = regionId;
                        physicianRegion.PhysicianId = physician.PhysicianId;
                        _context.PhysicianRegions.Add(physicianRegion);
                    }
                }
                _context.SaveChanges();

                AspNetUserRole aspNetUserRole = new AspNetUserRole();
                aspNetUserRole.UserId = aspNetUser.Id;
                aspNetUserRole.RoleId = 1;
                _context.AspNetUserRoles.Add(aspNetUserRole);
                _context.SaveChanges();
                string filepath = "physician/" + physician.PhysicianId;
                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", filepath);
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "photo." + model.Photo.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.Photo.CopyToAsync(stream)
;
                    }
                }
                if (model.ICA != null && model.ICA.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "ICA." + model.ICA.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.ICA.CopyToAsync(stream);
                    }
                }
                if (model.HIPAACompliance != null && model.HIPAACompliance.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "HIPAACompliance." + model.HIPAACompliance.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.HIPAACompliance.CopyToAsync(stream);
                    }
                }
                if (model.BackgroundCheck != null && model.BackgroundCheck.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "BackgroundCheck." + model.BackgroundCheck.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.BackgroundCheck.CopyToAsync(stream);
                    }
                }
                if (model.NonDisclosureAgreement != null && model.NonDisclosureAgreement.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, "NonDisclosureAgreement." + model.NonDisclosureAgreement.FileName.Split(".")[1]);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        model.NonDisclosureAgreement.CopyToAsync(stream);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public CreateProviderViewModel createPhysician()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 4;
            List<Region> regions = _context.Regions.ToList();
            List<Role> roles = _context.Roles.ToList();
            CreateProviderViewModel createProviderViewModel = new CreateProviderViewModel();
            createProviderViewModel.adminNavbarViewModel = adminNavbarViewModel;
            //var PhysicianDetails = _context.Admins.FirstOrDefault(u => u.AspNetUserId == aspNetUserId);

            createProviderViewModel.Regions = regions;
            createProviderViewModel.Roles = roles;

            //createProviderViewModel.State = regions;
            createProviderViewModel.check = false;
            return createProviderViewModel;

        }
        public bool editBusiness(AddBusinessViewModel model)
        {
            try
            {
                HealthProfessional healthProfessional = _context.HealthProfessionals.FirstOrDefault(i => i.VendorId == model.healthProfessionId);
                Region region = _context.Regions.FirstOrDefault(i => i.RegionId == model.State);
                healthProfessional.Profession = model.ProfessionType;
                healthProfessional.Address = model.Street;
                healthProfessional.City = model.City;
                healthProfessional.State = region.Name;
                healthProfessional.Zip = model.Zipcode;
                healthProfessional.VendorName = model.BusinessName;
                healthProfessional.BusinessContact = model.BusinessContact;
                healthProfessional.FaxNumber = model.FaxNumber;
                healthProfessional.RegionId = model.State;
                healthProfessional.PhoneNumber = model.PhoneNumber;
                healthProfessional.Email = model.Email;
                healthProfessional.ModifiedDate = DateTime.Now;
                _context.HealthProfessionals.Update(healthProfessional);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public AddBusinessViewModel addBusiness()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 5;
            List<HealthProfessionalType> healthProfessionalTypes = _context.HealthProfessionalTypes.ToList();
            List<Region> regions = _context.Regions.ToList();
            AddBusinessViewModel addBusinessViewModel = new AddBusinessViewModel();
            addBusinessViewModel.adminNavbarViewModel = adminNavbarViewModel;
            addBusinessViewModel.ProfessionTypes = healthProfessionalTypes;
            addBusinessViewModel.States = regions;
            addBusinessViewModel.Header = "Add Business";
            return addBusinessViewModel;
        }
        public AddBusinessViewModel editBusinessPage(int id)
        {
            BitArray check = new BitArray(1);
            check.Set(0, false);
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 5;
            List<Region> regions = _context.Regions.ToList();
            List<HealthProfessionalType> healthProfessionalTypes = _context.HealthProfessionalTypes.ToList();
            HealthProfessional healthProfessional = _context.HealthProfessionals.FirstOrDefault(i => i.VendorId == id && i.IsDeleted == check);
            AddBusinessViewModel addBusinessViewModel = new AddBusinessViewModel();
            addBusinessViewModel.Street = healthProfessional.Address;
            addBusinessViewModel.City = healthProfessional.City;
            addBusinessViewModel.State = (int)healthProfessional.RegionId;
            addBusinessViewModel.Zipcode = healthProfessional.Zip;
            addBusinessViewModel.ProfessionType = (int)healthProfessional.Profession;
            addBusinessViewModel.States = regions;
            addBusinessViewModel.Email = healthProfessional.Email;
            addBusinessViewModel.BusinessContact = healthProfessional.BusinessContact;
            addBusinessViewModel.BusinessName = healthProfessional.VendorName;
            addBusinessViewModel.ProfessionTypes = healthProfessionalTypes;
            addBusinessViewModel.PhoneNumber = healthProfessional.PhoneNumber;
            addBusinessViewModel.FaxNumber = healthProfessional.FaxNumber;
            addBusinessViewModel.adminNavbarViewModel = adminNavbarViewModel;
            addBusinessViewModel.Header = "Edit Business";
            addBusinessViewModel.healthProfessionId = healthProfessional.VendorId;
            return addBusinessViewModel;
        }
        public bool addBusiness(AddBusinessViewModel model)
        {
            try
            {
                BitArray check = new BitArray(1);
                check.Set(0, false);
                Region region = _context.Regions.FirstOrDefault(i => i.RegionId == model.State);
                HealthProfessional healthProfessional = new HealthProfessional();
                healthProfessional.Profession = model.ProfessionType;
                healthProfessional.Address = model.Street;
                healthProfessional.City = model.City;
                healthProfessional.State = region.Name;
                healthProfessional.Zip = model.Zipcode;
                healthProfessional.VendorName = model.BusinessName;
                healthProfessional.BusinessContact = model.BusinessContact;
                healthProfessional.FaxNumber = model.FaxNumber;
                healthProfessional.RegionId = model.State;
                healthProfessional.PhoneNumber = model.PhoneNumber;
                healthProfessional.IsDeleted = check;
                healthProfessional.Email = model.Email;
                healthProfessional.CreatedDate = DateTime.Now;
                _context.HealthProfessionals.Add(healthProfessional);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public ProviderLocationViewModel providerLocation()
        {
            ProviderLocationViewModel providerLocationViewModel = new ProviderLocationViewModel();
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 2;
            providerLocationViewModel.adminNavbarViewModel = adminNavbarViewModel;
            List<PhysicianLocation> physicianLocations = _context.PhysicianLocations.ToList();
            providerLocationViewModel.Query = physicianLocations;
            return providerLocationViewModel;
        }
        public ProviderOnCallViewModel providerOnCall(int? region)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 13;

            var currentDateTime = DateTime.Now;
            var allPhysicians = _context.Physicians.Where(item => item.IsDeleted == new BitArray(1, false)).Select(p => new { p.FirstName, p.LastName, p.PhysicianId }).ToList();

            var query = from sh in _context.Shifts
                        join sd in _context.ShiftDetails on sh.ShiftId equals sd.ShiftId
                        where sd.ShiftDate == currentDateTime.Date
                              && TimeOnly.FromTimeSpan(currentDateTime.TimeOfDay) >= sd.StartTime
                              && TimeOnly.FromTimeSpan(currentDateTime.TimeOfDay) <= sd.EndTime
                              && (region == null || region == -1 || sd.RegionId == region)
                        select new { PhysicianName = $"{sh.Physician.FirstName} {sh.Physician.LastName}", sh.PhysicianId };

            var physicianExceptions = query.Select(x => x.PhysicianId).ToList();
            var excludedPhysician = new { PhysicianId = query.Select(i => i.PhysicianId) };
            List<PhysicianProfile> offDutyPhysicians = allPhysicians.Select(i => new PhysicianProfile { PhysicianId = i.PhysicianId, PhysicianName = i.FirstName + " " + i.LastName }).Where(i => !excludedPhysician.PhysicianId.Contains(i.PhysicianId)).ToList();
            List<PhysicianProfile> onCallPhysicians = query.Select(x => new PhysicianProfile
            {
                PhysicianName = x.PhysicianName,
                PhysicianId = x.PhysicianId
            }).ToList();
            List<Region> regions = _context.Regions.ToList();
            ProviderOnCallViewModel providerOnCallViewModel = new ProviderOnCallViewModel();
            providerOnCallViewModel.adminNavbarViewModel = adminNavbarViewModel;
            providerOnCallViewModel.MDOnCall = onCallPhysicians;
            providerOnCallViewModel.PhysicianOffDuty = offDutyPhysicians;
            providerOnCallViewModel.Regions = regions;
            return providerOnCallViewModel;
        }
    }
}
