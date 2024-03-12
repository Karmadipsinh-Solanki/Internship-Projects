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
        //AdminDashboardTableView IAdmin.adminDashboard(string status, string? search, string? requestor, int? region, int page = 1, int pageSize = 10)
        //{

        //}

        AdminDashboardTableView IAdmin.adminDashboard(string status,string? search, int? region, string? requestor)
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
                query = query.Where(r => r.RequestClient.FirstName.Contains(search) || r.RequestClient.LastName.Contains(search));
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
            if (region != null && region != -1)
            {
                query = query.Where(r => r.RequestClient.RegionId == region);

            }


            AdminDashboardTableView adminDashboardViewModel = new AdminDashboardTableView
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                query_requests = query,
                requests = query.ToList(),
                regions = regions,
                status = status,
            };
            return adminDashboardViewModel;
        }
        bool IAdmin.viewNotes(ViewNotesViewModel model)
        {
            //int aspnetuserid = (int)_context.HttpContext.Session.GetInt32("AspNetUserId");
            //int aspnetuserid = (int)_context.HttpContext.Session.GetInt32("AspNetUserId");//

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
            return true;
           
        }
        ViewNotesViewModel IAdmin.viewNotes(int id)
        {
            var patientcancel = _context.RequestStatusLogs.FirstOrDefault(r => r.RequestId == id && r.Status == 7);
            var admincancel = _context.RequestStatusLogs.FirstOrDefault(r => r.RequestId == id && r.Status == 6);
            var transfernotes = _context.RequestStatusLogs.Where(r => r.RequestId == id && r.Status == 2).ToList();
            var requestnotes = _context.RequestNotes.FirstOrDefault(r => r.RequestId == id);


            //var adminid = _context.httpcontext.session.getint32("adminid");
            //var adminid = httpcontext.session.getint32("adminid");
            //var admin = _context.admins.firstordefault(a => a.adminid == adminid);

            //adminnavbarviewmodel adminnavbarviewmodel = new adminnavbarviewmodel//
            //{
            //    name = string.concat(admin.firstname, " ", admin.lastname),
            //    curr_active = "dashboard",
            //};

            ViewNotesViewModel viewnotesviewmodel = new ViewNotesViewModel
            {
                RequestId = id,
                Admin_Note = requestnotes?.AdminNotes ?? "-",
                Physician_Note = requestnotes?.PhysicianNotes ?? "-",
                Admin_Cancellation_Note = admincancel?.Notes,
                Cancellation_Note = patientcancel?.Notes,
                Transfer_Notes = transfernotes,
            };
            return viewnotesviewmodel;
        }
        ViewCaseModel IAdmin.viewCase(int id)
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
          return viewCaseModel;
            
        }
        bool IAdmin.viewCase(ViewCaseModel model)
        {
            var requestId = model.RequestId;
            if (requestId != null)
            {
                var rid = _context.Requests.Where(u => u.RequestId == requestId).FirstOrDefault();
                var userToUpdate = _context.RequestClients.Where(u => u.RequestClientId == rid.RequestClientId).FirstOrDefault();
                if (userToUpdate != null)
                {
                    userToUpdate.FirstName = model.FirstName;
                    userToUpdate.LastName = model.LastName;
                    userToUpdate.PhoneNumber = model.PhoneNumber;
                    userToUpdate.Email = model.Email;
                    userToUpdate.IntDate = model.DOB.Day;
                    userToUpdate.IntYear = model.DOB.Year;
                    userToUpdate.StrMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.DOB.Month);
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
        MemoryStream IAdmin.downloadExcel()
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
        bool IAdmin.sendLink(AdminDashboardTableView model)
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
        int IAdmin.createRequest(CreateRequestViewModel model)
        {
            AspNetUser aspNetUser = new AspNetUser();
            User user = new User();
            Request request = new Request();
            RequestClient requestClient = new RequestClient();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();

            var region = _context.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));
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
                        Subject = "Register Case",
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
            requestClient.RegionId = 1;
            if (model.Notes != null)
            {
                requestClient.Notes = model.Notes;
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
            requestStatusLog.Notes = model.Notes;
            requestStatusLog.CreatedDate = DateTime.Now;
            _context.RequestStatusLogs.Add(requestStatusLog);
            _context.SaveChanges();
            return 2;
        }
        HealthProfessional IAdmin.fetchBusinessDetail(int id)
        {
            return _context.HealthProfessionals.FirstOrDefault(r => r.VendorId == id);
        }
        List<HealthProfessional> IAdmin.fetchBusiness(int id)
        {
            return _context.HealthProfessionals.Where(r => r.Profession == id).ToList();
        }

        List<Region> IAdmin.fetchRegions()
        {
            return _context.Regions.ToList(); // Directly project to List<Region>
        }
        List<Physician> IAdmin.fetchPhysicians(int id)
        {
            return _context.Physicians.Where(r => r.RegionId == id).ToList();
        }
        List<CaseTag> IAdmin.fetchTags()
        {
            return _context.CaseTags.ToList();
        }
        bool IAdmin.assignCase(AdminDashboardTableView model)
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
        bool IAdmin.cancelCase(AdminDashboardTableView model)
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
        bool IAdmin.clearCase(AdminDashboardTableView model)
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
        bool IAdmin.closeCase(AdminDashboardTableView model)
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
        bool IAdmin.blockCase(AdminDashboardTableView model)
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
        bool IAdmin.transferCase(AdminDashboardTableView model)
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
        ViewUploadViewModel IAdmin.viewUpload(int id)
        {
            //to save file in wwwroot,that is uploaded by patient
            //token
            var request = _context.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            var documents = _context.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id).ToList();
            var user = _context.Users.FirstOrDefault(u => u.UserId == request.UserId);
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            ViewUploadViewModel viewUploadViewModel = new ViewUploadViewModel();
            //AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            //adminNavbarViewModel.AdminName = AdminName;
            viewUploadViewModel.patient_name = string.Concat(request.RequestClient.FirstName, ' ', request.RequestClient.LastName);
            viewUploadViewModel.name = string.Concat(user.FirstName, ' ', user.LastName);
            viewUploadViewModel.confirmation_number = request.ConfirmationNumber;
            viewUploadViewModel.requestWiseFiles = documents;
            //viewUploadViewModel.uploader_name = string.Concat(request.FirstName, ' ', request.LastName);
            viewUploadViewModel.RequestId = id;
            //viewUploadViewModel.adminNavbarViewModel = adminNavbarViewModel;
            return viewUploadViewModel;
        }
        bool IAdmin.viewUpload(ViewUploadViewModel model)
        {
            if (model.File != null && model.File.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", model.File.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    model.File.CopyTo(stream);
                }
                RequestWiseFile requestWiseFile = new RequestWiseFile();
                requestWiseFile.RequestId = (int)model.RequestId;
                //IformFile has a property that to store filepath u need to add .filename behind it to store path
                //requestWiseFile.AdminId = 1;
                requestWiseFile.FileName = model.File.FileName;
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
         ViewUploadViewModel IAdmin.closeCase(int id)
        {
            //to save file in wwwroot,that is uploaded by patient
            //token
            var request = _context.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            var documents = _context.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id).ToList();
            var user = _context.Users.FirstOrDefault(u => u.UserId == request.UserId);
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;
            ViewUploadViewModel viewUploadViewModel = new ViewUploadViewModel();
            //AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel();
            //adminNavbarViewModel.AdminName = AdminName;
            viewUploadViewModel.patient_name = string.Concat(request.RequestClient.FirstName, ' ', request.RequestClient.LastName);
            viewUploadViewModel.name = string.Concat(user.FirstName, ' ', user.LastName);
            viewUploadViewModel.confirmation_number = request.ConfirmationNumber;
            viewUploadViewModel.requestWiseFiles = documents;
            viewUploadViewModel.RequestId = id;
            //viewUploadViewModel.adminNavbarViewModel = adminNavbarViewModel;
            return viewUploadViewModel;
        }
        bool IAdmin.closeCase(ViewUploadViewModel model)
        {
            if (model.File != null && model.File.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", model.File.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    model.File.CopyTo(stream);
                }
                RequestWiseFile requestWiseFile = new RequestWiseFile();
                requestWiseFile.RequestId = (int)model.RequestId;
                //IformFile has a property that to store filepath u need to add .filename behind it to store path
                //requestWiseFile.AdminId = 1;
                requestWiseFile.FileName = model.File.FileName;
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
        OrderViewModel IAdmin.SendOrder(int id)
        {
            var healthProfessionalType = _context.HealthProfessionalTypes.ToList();
            var healthProfessional = _context.HealthProfessionals.ToList();
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            string AdminName = cookieModel.name;

            OrderViewModel orderViewModel = new OrderViewModel();
            orderViewModel.RequestId = id;
            orderViewModel.healthProfessionalTypes = healthProfessionalType;
            orderViewModel.healthProfessionals = healthProfessional;
            return orderViewModel;
        }
        bool IAdmin.SendOrder(OrderViewModel model)
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
    }
}
