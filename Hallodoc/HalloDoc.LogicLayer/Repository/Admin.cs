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

namespace HalloDoc.LogicLayer.Repository
{
    public class Admin : IAdmin
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;

        public Admin(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
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
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            adminNavbarViewModel.AdminName = AdminName;
            adminNavbarViewModel.Tab = 1;

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
            catch(Exception ex)
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
        public bool verifyState(CreateRequestViewModel model)
        {
            var details = _context.Requests.FirstOrDefault(i => i.RequestId == model.RequestId);
            Region region = _context.Regions.FirstOrDefault(u => u.Name.Trim().ToLower().Replace(" ", "") == model.State.Trim().ToLower().Replace(" ", ""));
            if (region != null)
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
        public List<Physician> fetchPhysicians(int id)
        {
            return _context.Physicians.Where(r => r.RegionId == id).ToList();
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
                var requestToUpdate = _context.Requests.Where(u => u.RequestId == requestId).FirstOrDefault();

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
                blockRequest.RequestId = (model.RequestId).ToString();
                blockRequest.Reason = model.BlockReason;
                blockRequest.Email = requestToUpdate.Email;
                blockRequest.PhoneNumber = requestToUpdate.PhoneNumber;
                blockRequest.CreatedDate = DateTime.Now;
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
        public ViewUploadViewModel closeCase(int id)/////
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

            var existingUser = _context.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
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
                var userFirstName = model.FirstName + " " + model.LastName;
                var formatedDate = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                string reviewAgreement = $"https://localhost:44339/Login/ReviewAgreement";
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
                Hallodoc.Admin admin = _context.Admins.FirstOrDefault(u => u.AspNetUserId == aspNetUserId);
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
                Hallodoc.Admin admin = _context.Admins.FirstOrDefault(u => u.AspNetUserId == aspNetUserId);
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
                Hallodoc.Admin admin = new Hallodoc.Admin();

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
        CreateAdminViewModel IAdmin.createAdmin()
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
    }
}
