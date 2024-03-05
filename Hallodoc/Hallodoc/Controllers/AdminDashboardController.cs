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
//using System.Drawing;
using System.Linq;
using HalloDoc.ViewModels;
using System.Net.Mail;
using System.Net;
using System.Collections;
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
            var count_active = _context.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _context.Requests.Count(r => r.Status == 5);
            var count_toclose = _context.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _context.Requests.Count(r => r.Status == 9);

            var caseTag = _context.CaseTags.ToList();
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
                caseTags = caseTag,
            };

            return View(adminDashboardViewModel);
        }

        public IActionResult New(string? search, int? region, string? requestor)
        {
            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _context.Requests.Count(r => r.Status == 5);
            var count_toclose = _context.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _context.Requests.Count(r => r.Status == 9);

            IQueryable<Request> query = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 1);
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
                status = "New",
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardViewModel);
        }

        public IActionResult Pending(string? search, int? region, string? requestor)
        {
            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _context.Requests.Count(r => r.Status == 5);
            var count_toclose = _context.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _context.Requests.Count(r => r.Status == 9);

            IQueryable<Request> query = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 2);
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
                status = "Pending",
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardViewModel);
        }

        public IActionResult Active(string? search, int? region, string? requestor)
        {
            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _context.Requests.Count(r => r.Status == 5);
            var count_toclose = _context.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _context.Requests.Count(r => r.Status == 9);

            IQueryable<Request> query = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 3 || r.Status == 4);
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
                status = "Active",
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardViewModel);
        }

        public IActionResult Conclude(string? search, int? region, string? requestor)
        {
            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _context.Requests.Count(r => r.Status == 5);
            var count_toclose = _context.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _context.Requests.Count(r => r.Status == 9);

            IQueryable<Request> query = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 5);
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
                status = "Conclude",
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardViewModel);
        }

        public IActionResult Toclose(string? search, int? region, string? requestor)
        {
            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _context.Requests.Count(r => r.Status == 5);
            var count_toclose = _context.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _context.Requests.Count(r => r.Status == 9);

            IQueryable<Request> query = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
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
                status = "ToClose",
            };
            return PartialView("AdminDashboardTablePartialView", adminDashboardViewModel);
        }

        public IActionResult Unpaid(string? search, int? region, string? requestor)
        {
            var count_new = _context.Requests.Count(r => r.Status == 1);
            var count_pending = _context.Requests.Count(r => r.Status == 2);
            var count_active = _context.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _context.Requests.Count(r => r.Status == 5);
            var count_toclose = _context.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _context.Requests.Count(r => r.Status == 9);

            IQueryable<Request> query = _context.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 9);
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

        [HttpPost]
        public async Task<IActionResult> SendLink(AdminDashboardTableView model)
        {
            AspNetUser aspNetUser = new AspNetUser();
            User user = new User();
            Request request = new Request();
            RequestClient requestClient = new RequestClient();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();

            var blockedUser = _context.BlockRequests.FirstOrDefault(u => u.Email == model.Email);
            if (blockedUser != null)
            {
                ModelState.AddModelError("Email", "This patient is blocked.");
                return View(model);
            }

            var existingUser = _context.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
            var id = _context.Users.SingleOrDefault(u => u.Email == model.Email);
            bool userExists = true;
            if (existingUser == null)
            {
                userExists = false;
                aspNetUser.UserName = model.Email;
                aspNetUser.Email = model.Email;
                //aspNetUser.PhoneNumber = model.PhoneNumber;
                aspNetUser.CreatedDate = DateTime.Now;
                _context.AspNetUsers.Add(aspNetUser);
                await _context.SaveChangesAsync();

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
            return RedirectToAction("AdminDashboard");
        }
        public IActionResult CreateRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(CreateRequestViewModel model)
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
                ModelState.AddModelError("State", "Currently we are not serving in this region");
                return View(model);
            }
            var blockedUser = _context.BlockRequests.FirstOrDefault(u => u.Email == model.Email);
            if (blockedUser != null)
            {
                ModelState.AddModelError("Email", "This patient is blocked.");
                return View(model);
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
                await _context.SaveChangesAsync();

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
                await _context.SaveChangesAsync();
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
            await _context.SaveChangesAsync();

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
            await _context.SaveChangesAsync();

            requestStatusLog.RequestId = request.RequestId;
            requestStatusLog.Status = 1;
            requestStatusLog.Notes = model.Notes;
            requestStatusLog.CreatedDate = DateTime.Now;
            _context.RequestStatusLogs.Add(requestStatusLog);
            await _context.SaveChangesAsync();
            TempData["success"] = "Request created successfully!";
            return RedirectToAction("AdminDashboard");
        }


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
            return View(viewnotesviewmodel);
        }

        //bool IAdmin.updateAdminNotes(ViewNotesViewModel viewNotesViewModel)
        [HttpPost]
        public IActionResult ViewNotes(ViewNotesViewModel viewNotesViewModel)
        {
            //int aspnetuserid = (int)_context.HttpContext.Session.GetInt32("AspNetUserId");
            //int aspnetuserid = (int)_context.HttpContext.Session.GetInt32("AspNetUserId");//

            RequestNote requestNote = _context.RequestNotes.FirstOrDefault(r => r.RequestId == viewNotesViewModel.RequestId);
            if (requestNote != null)
            {
                requestNote.AdminNotes = viewNotesViewModel.Admin_Note;
                requestNote.ModifiedDate = DateTime.Now;
                _context.RequestNotes.Update(requestNote);
                _context.SaveChanges();
            }
            else
            {
                RequestNote newRequestNote = new RequestNote
                {
                    RequestId = viewNotesViewModel.RequestId,
                    AdminNotes = viewNotesViewModel.Admin_Note,
                    CreatedDate = DateTime.Now,
                    CreatedBy = 2,
                };

                _context.RequestNotes.Add(newRequestNote);
                _context.SaveChanges();
            }
            return RedirectToAction("ViewNotes", new { id = viewNotesViewModel.RequestId });

        }
        public IActionResult FetchTags()
        {
            // Replace with your actual logic to fetch regions
            var regions = _context.CaseTags.Select(r => new { Id = r.CaseTagId, Name = r.Name }).ToList();
            return Json(regions);
        }
        public IActionResult FetchRegions()
        {
            var regions = _context.Regions.Select(r => new { Id = r.RegionId, Name = r.Name }).ToList();
            return Json(regions);
        }
        public IActionResult FetchPhysicians(int id)
        {
            var physicians = _context.Physicians.Where(r => r.RegionId == id).ToList();
            return Json(physicians);
        }

        public async Task<IActionResult> AssignCase(AdminDashboardTableView model)
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
                    await _context.SaveChangesAsync();
                }
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.RequestId = requestId;
                requestStatusLog.Status = 2;
                requestStatusLog.Notes = "Admin transferred to Dr. " + PhysicianName + " on " + DateTime.Now.ToString("dd/MM/yyyy") + " at " + DateTime.Now.ToString("HH:mm:ss") + " : " + model.Description;
                requestStatusLog.CreatedDate = DateTime.Now;
                requestStatusLog.TransToPhysicianId = model.PhysicianId;
                _context.RequestStatusLogs.Add(requestStatusLog);
                await _context.SaveChangesAsync();
                TempData["success"] = "Case assigned successfully!";
            }
            return RedirectToAction("AdminDashboard");
        }

        [HttpPost]
        public IActionResult CancelCase(AdminDashboardTableView model)
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
            }
            return RedirectToAction("AdminDashboard");
        }
        [HttpPost]
        public async Task<IActionResult> BlockCase(AdminDashboardTableView model)
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
                    await _context.SaveChangesAsync();
                }
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.RequestId = requestId;
                requestStatusLog.Status = 11;
                requestStatusLog.Notes = model.BlockReason;
                requestStatusLog.CreatedDate = DateTime.Now;
                _context.RequestStatusLogs.Add(requestStatusLog);
                await _context.SaveChangesAsync();

                BlockRequest blockRequest = new BlockRequest();
                blockRequest.RequestId = (model.RequestId).ToString();
                blockRequest.Reason = model.BlockReason;
                blockRequest.Email = requestToUpdate.Email;
                blockRequest.PhoneNumber = requestToUpdate.PhoneNumber;
                blockRequest.CreatedDate = DateTime.Now;
                _context.BlockRequests.Add(blockRequest);
                await _context.SaveChangesAsync();
                TempData["success"] = "Case blocked successfully!";
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
        public async Task<IActionResult> ViewUpload(int id)
        {
            //to save file in wwwroot,that is uploaded by patient

            var user_id = HttpContext.Session.GetInt32("id");
            var request = _context.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            //var request = _viewDoc.ListOfIncludeAdminPhysicianToReq(id);
            var documents = _context.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id && u.IsDeleted.Equals(new BitArray(new[] { false }))).ToList();
            //var documents = _viewDoc.ListOfIncludeAdminPhysicianToReqwisefile(id);

            var user = _context.Users.FirstOrDefault(u => u.UserId == request.UserId);
            ViewUploadViewModel viewUploadViewModel = new ViewUploadViewModel()
            {
                patient_name = string.Concat(request.RequestClient.FirstName, ' ', request.RequestClient.LastName),
                name = string.Concat(user.FirstName, ' ', user.LastName),
                confirmation_number = request.ConfirmationNumber,
                requestWiseFiles = documents,
                //uploader_name = string.Concat(request.FirstName, ' ', request.LastName),
                RequestId = id,
                //partialViewModel = new partialViewModel() { patient_name = string.Concat(user.FirstName + ' ' + user.LastName) },
            };
            return View(viewUploadViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ViewUpload(ViewUploadViewModel model, int id)
        {
            if (model.File != null && model.File.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", model.File.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.File.CopyToAsync(stream);
                }
                RequestWiseFile requestWiseFile = new RequestWiseFile();
                requestWiseFile.RequestId = id;
                //IformFile has a property that to store filepath u need to add .filename behind it to store path
                //requestWiseFile.AdminId = 1;
                requestWiseFile.FileName = model.File.FileName;
                requestWiseFile.CreatedDate = DateTime.Now;
                //ishan
                _context.RequestWiseFiles.Add(requestWiseFile);
                await _context.SaveChangesAsync();
                //Ishan
                return RedirectToAction("ViewUpload", new { id = id });
            }
            else
            {
                return RedirectToAction("ViewUpload", new { id = id });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
