using Hallodoc;
using HalloDoc.DataLayer.ViewModels;
using HalloDoc.LogicLayer.Interface;
using HalloDoc.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HalloDoc.DataLayer.Data;
using HalloDoc.DataLayer.Models;

namespace HalloDoc.LogicLayer.Repository
{
    public class CreateReq : ICreateReq
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;

        public CreateReq(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
        }
        bool ICreateReq.createPassword(CreatePasswordViewModel model)
        {
            var aspnetUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == model.email);
            if (aspnetUser != null)
            {
                var passwordHasher = new PasswordHasher<AspNetUser>();
                aspnetUser.PasswordHash = passwordHasher.HashPassword(aspnetUser, model.password);
                _context.AspNetUsers.Update(aspnetUser);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public int createPatientRequest(RequestViewModelPatient model)
        {
            AspNetUser aspNetUser = new AspNetUser();
            User user = new User();
            RequestClient requestClient = new RequestClient();
            Request request = new Request();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();
            Concierge concierge = new Concierge();
            RequestConcierge requestConcierge = new RequestConcierge();
            AspNetUserRole aspNetUserRole = new AspNetUserRole();

            //to add one more state,that is to show that we dont give service in particular region
            var region = _context.Regions.FirstOrDefault(u => u.Name.Trim().ToLower().Replace(" ", "") == model.State.Trim().ToLower().Replace(" ", ""));
            if (region == null)
            {
                return 0;
            }
            //for block request
            var blockedUser = _context.BlockRequests.FirstOrDefault(u => u.Email == model.Email);
            if (blockedUser != null)
            {
                return 1;
            }
            //to save file in wwwroot,that is uploaded by patient
            if (model.File != null && model.File.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", model.File.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    model.File.CopyTo(stream);
                }
            }

            var existingUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);
            //var existingUser = _createPatientRequest.EmailFromAspnetuser(model);
            bool userExists = true;

            if (existingUser == null)
            {
                userExists = false;
                if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName))
                {
                    aspNetUser.UserName = "GuestUser";
                }
                else
                {
                    aspNetUser.UserName = model.FirstName + " " + model.LastName;
                }
                aspNetUser.Email = model.Email;
                aspNetUser.PhoneNumber = model.PhoneNumber;
                aspNetUser.CreatedDate = DateTime.Now;
                aspNetUser.PasswordHash = model.PasswordHash;
                //aspNetUser.UserName = model.FirstName + " " + model.LastName;
                //ishan
                _context.AspNetUsers.Add(aspNetUser);
                _context.SaveChanges();
                //ishan

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

                aspNetUserRole.UserId = aspNetUser.Id;
                aspNetUserRole.RoleId = 2;
                _context.AspNetUserRoles.Add(aspNetUserRole);
                _context.SaveChanges();
            }

            requestClient.FirstName = model.FirstName;
            requestClient.LastName = model.LastName;
            requestClient.PhoneNumber = model.PhoneNumber;
            requestClient.Location = model.City;
            requestClient.Address = model.Street;
            requestClient.RegionId = region.RegionId;
            if (model.Symptoms != null)
            {
                requestClient.Notes = model.Symptoms;
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

            //to generate confirmation number(method is given in srs that how to generate confirmation number
            int requests = _context.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();
            string ConfirmationNumber = string.Concat(region.Abbreviation, model.FirstName.Substring(0, 2).ToUpper(), model.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));
            //
            request.RequestTypeId = 1;
            if (!userExists)
            {
                request.UserId = user.UserId;
            }
            request.FirstName = model.FirstName;
            request.LastName = model.LastName;
            request.Email = model.Email;
            request.PhoneNumber = model.PhoneNumber;
            request.Status = 1;
            request.ConfirmationNumber = ConfirmationNumber;
            request.CreatedDate = DateTime.Now;
            //RequestId dropped from requestClient and requestClientId added in request + foreign key
            request.RequestClientId = requestClient.RequestClientId;
            _context.Requests.Add(request);
            _context.SaveChanges();

            if (model.File != null)
            {
                requestWiseFile.RequestId = request.RequestId;
                //IformFile has a property that to store filepath u need to add .filename behind it to store path
                requestWiseFile.FileName = model.File.FileName;
                requestWiseFile.CreatedDate = DateTime.Now;
                _context.RequestWiseFiles.Add(requestWiseFile);
                _context.SaveChanges();

            }

            requestStatusLog.RequestId = request.RequestId;
            requestStatusLog.Status = 1;
            requestStatusLog.Notes = model.Symptoms;
            requestStatusLog.CreatedDate = DateTime.Now;
            _context.RequestStatusLogs.Add(requestStatusLog);
            _context.SaveChanges();
            return 2;
        }
        public int createConciergeRequest(RequestViewModelConcierge model)
        {
            AspNetUser aspNetUser = new AspNetUser();
            User user = new User();
            Request request = new Request();
            RequestClient requestClient = new RequestClient();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();
            Concierge concierge = new Concierge();
            RequestConcierge requestConcierge = new RequestConcierge();
            AspNetUserRole aspNetUserRole = new AspNetUserRole();

            var region = _context.Regions.FirstOrDefault(u => u.Name.Trim().ToLower().Replace(" ", "") == model.CState.Trim().ToLower().Replace(" ", ""));
            if (region == null)
            {
                return 0;
            }
            var blockedUser = _context.BlockRequests.FirstOrDefault(u => u.Email == model.Email);
            if (blockedUser != null)
            {
                return 1;
            }

            var existingUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);
            var id = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            bool userExists = true;
            if (existingUser == null)
            {
                userExists = false;
                aspNetUser.UserName = model.Email;
                aspNetUser.Email = model.Email;
                aspNetUser.PhoneNumber = model.PhoneNumber;
                aspNetUser.CreatedDate = DateTime.Now;
                aspNetUser.PasswordHash = model.Password;
                _context.AspNetUsers.Add(aspNetUser);
                _context.SaveChanges();

                user.AspNetUserId = aspNetUser.Id;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Mobile = model.PhoneNumber;
                user.Street = model.CStreet;
                user.City = model.CCity;
                user.State = model.CState;
                user.ZipCode = model.CZipCode;
                user.IntDate = model.DOB.Day;
                user.StrMonth = model.DOB.Month.ToString();
                user.IntYear = model.DOB.Year;
                user.CreatedBy = aspNetUser.Id;
                user.CreatedDate = DateTime.Now;
                _context.Users.Add(user);
                _context.SaveChanges();

                aspNetUserRole.UserId = aspNetUser.Id;
                aspNetUserRole.RoleId = 2;
                _context.AspNetUserRoles.Add(aspNetUserRole);
                _context.SaveChanges();

                var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
                {
                    Credentials = new NetworkCredential("c3f46c2b681459", "d860d68bf5a0db"),
                    EnableSsl = true
                };
                string email = model.Email;
                var aspnetUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == email);
                var userdb = _context.Users.FirstOrDefault(u => u.Email == email);
                var userFirstName = userdb.FirstName;
                //string date = new DateTime.Now;
                var formatedDate = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                string resetLink = $"https://localhost:44379/PatientRequest/CreatePassword?email={email}";
                string message = $@"<html>
                                <body>  
                                <h1>Create password request</h1>  
                                <h2>Hii {userFirstName},</h2>
                                <p style=""margin-top:30px;"">In order to create your account we need your password,so please click the below link to create password:</p>
                                <p><a href=""{resetLink}"">Create Password</a></p> 
                                <p>If you didn't request an account creation then please ignore this mail.</p>
                                </body>
                                </html>";
                if (aspnetUser != null)
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("hallodoc@gmail.com"),
                        Subject = "Create Password for HalloDoc account",
                        Body = message,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(email);
                    client.Send(mailMessage);
                }
            }
            requestClient.FirstName = model.FirstName;
            requestClient.LastName = model.LastName;
            requestClient.PhoneNumber = model.PhoneNumber;
            requestClient.Location = model.CCity;
            requestClient.Address = model.CStreet;
            requestClient.RegionId = region.RegionId;
            requestClient.Notes = model.Symptoms;
            requestClient.Email = model.Email;
            requestClient.IntDate = model.DOB.Day;
            requestClient.StrMonth = model.DOB.Month.ToString();
            requestClient.IntYear = model.DOB.Year;
            requestClient.Street = model.CStreet;
            requestClient.City = model.CCity;
            requestClient.State = model.CState;
            requestClient.ZipCode = model.CZipCode;
            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();



            int requests = _context.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();
            string ConfirmationNumber = string.Concat(region.Abbreviation, model.FirstName.Substring(0, 2).ToUpper(), model.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));
            request.RequestTypeId = 3;
            if (!userExists)
            {
                request.UserId = user.UserId;
            }
            else
            {
                request.UserId = id.UserId;
            }
            request.FirstName = model.CFirstName;
            request.LastName = model.CLastName;
            request.Email = model.CEmail;
            request.PhoneNumber = model.CPhoneNumber;
            request.Status = 1;
            request.CreatedDate = DateTime.Now;
            request.RequestClientId = requestClient.RequestClientId;
            request.ConfirmationNumber = ConfirmationNumber;
            _context.Requests.Add(request);
            _context.SaveChanges();

            requestStatusLog.RequestId = request.RequestId;
            requestStatusLog.Status = 1;
            requestStatusLog.Notes = model.Symptoms;
            requestStatusLog.CreatedDate = DateTime.Now;
            _context.RequestStatusLogs.Add(requestStatusLog);
            _context.SaveChanges();

            concierge.ConciergeName = model.CFirstName;
            concierge.Address = model.CHotel;
            concierge.Street = model.CStreet;
            concierge.City = model.CCity;
            concierge.State = model.CState;
            concierge.ZipCode = model.CZipCode;
            concierge.CreatedDate = DateTime.Now;
            _context.Concierges.Add(concierge);
            _context.SaveChanges();

            requestConcierge.RequestId = request.RequestId;
            requestConcierge.ConciergeId = concierge.ConciergeId;
            _context.RequestConcierges.Add(requestConcierge);
            _context.SaveChanges();
            return 2;
        }
        public int createBusinessRequest(RequestViewModelBusiness model)
        {
            AspNetUser aspNetUser = new AspNetUser();
            User user = new User();
            Request request = new Request();
            RequestClient requestClient = new RequestClient();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();
            Business business = new Business();
            RequestBusiness requestBusiness = new RequestBusiness();
            AspNetUserRole aspNetUserRole = new AspNetUserRole();


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
            var existingUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);
            var id = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            bool userExists = true;
            if (existingUser == null)
            {
                userExists = false;
                aspNetUser.UserName = model.Email;
                aspNetUser.Email = model.Email;
                aspNetUser.PhoneNumber = model.PhoneNumber;
                aspNetUser.CreatedDate = DateTime.Now;
                aspNetUser.PasswordHash = model.Password;
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

                aspNetUserRole.UserId = aspNetUser.Id;
                aspNetUserRole.RoleId = 2;
                _context.AspNetUserRoles.Add(aspNetUserRole);
                _context.SaveChanges();


                var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
                {
                    Credentials = new NetworkCredential("c3f46c2b681459", "d860d68bf5a0db"),
                    EnableSsl = true
                };
                string email = model.Email;
                var aspnetUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == email);
                var userdb = _context.Users.FirstOrDefault(u => u.Email == email);
                var userFirstName = userdb.FirstName;

                string resetLink = $"https://localhost:44379/PatientRequest/CreatePassword?email={email}";
                string message = $@"<html>
                                <body>  
                                <h1>Create password request</h1> 
                                <h2>Hii {userFirstName},</h2>
                                <p style=""margin-top:30px;"">In order to create your account we need your password,so please click the below link to create password:</p>
                                <p><a href=""{resetLink}"">Create Password</a></p> 
                                <p>If you didn't request an account creation then please ignore this mail.</p>
                                </body>
                                </html>";
                if (aspnetUser != null)
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("hallodoc@gmail.com"),
                        Subject = "Create Password for HalloDoc account",
                        Body = message,
                        IsBodyHtml = true
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
            requestClient.Notes = model.Symptoms;
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
            request.FirstName = model.BFirstName;
            request.LastName = model.BLastName;
            request.Email = model.BEmail;
            request.PhoneNumber = model.BPhoneNumber;
            request.Status = 1;
            request.CreatedDate = DateTime.Now;
            request.RequestClientId = requestClient.RequestClientId;
            request.ConfirmationNumber = ConfirmationNumber;
            _context.Requests.Add(request);
            _context.SaveChanges();


            requestStatusLog.RequestId = request.RequestId;
            requestStatusLog.Status = 1;
            requestStatusLog.Notes = model.Symptoms;
            requestStatusLog.CreatedDate = DateTime.Now;
            _context.RequestStatusLogs.Add(requestStatusLog);
            _context.SaveChanges();

            business.Name = model.BFirstName + " " + model.BLastName;
            business.Address1 = model.BHotel;
            business.Address2 = model.BHotel;
            business.City = model.BHotel;
            business.ZipCode = "380058";
            business.CreatedDate = DateTime.Now;
            business.Status = 1;
            business.PhoneNumber = model.BPhoneNumber;
            _context.Businesses.Add(business);
            _context.SaveChanges();

            requestBusiness.RequestId = request.RequestId;
            requestBusiness.BusinessId = business.BusinessId;
            _context.RequestBusinesses.Add(requestBusiness);
            _context.SaveChanges();
            return 2;
        }
        public int createFamilyRequest(RequestViewModelFamily model)
        {
            AspNetUser aspNetUser = new AspNetUser();
            User user = new User();
            Request request = new Request();
            RequestClient requestClient = new RequestClient();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();
            AspNetUserRole aspNetUserRole = new AspNetUserRole();
            
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

            if (model.File != null && model.File.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", model.File.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    model.File.CopyTo(stream);
                }
            }


            var existingUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);

            bool userExists = true;
            if (existingUser == null)
            {
                userExists = false;
                aspNetUser.UserName = model.Email;
                aspNetUser.Email = model.Email;
                aspNetUser.PhoneNumber = model.PhoneNumber;
                aspNetUser.CreatedDate = DateTime.Now;
                aspNetUser.PasswordHash = model.Password;
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
                user.ZipCode = model.FZipCode;
                user.IntDate = model.DOB.Day;
                user.StrMonth = model.DOB.Month.ToString();
                user.IntYear = model.DOB.Year;
                user.CreatedBy = aspNetUser.Id;
                user.CreatedDate = DateTime.Now;
                _context.Users.Add(user);
                _context.SaveChanges();

                aspNetUserRole.UserId = aspNetUser.Id;
                aspNetUserRole.RoleId = 2;
                _context.AspNetUserRoles.Add(aspNetUserRole);
                _context.SaveChanges();

                var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
                {
                    Credentials = new NetworkCredential("c3f46c2b681459", "d860d68bf5a0db"),
                    EnableSsl = true
                };
                string email = model.Email;
                var aspnetUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == email);
                var userdb = _context.Users.FirstOrDefault(u => u.Email == email);
                var userFirstName = userdb.FirstName;
                string resetLink = $"https://localhost:44379/PatientRequest/CreatePassword?email={email}";
                string message = $@"<html>
                                <body>  
                                <h1>Create password request</h1>  
                                <h2>Hii {userFirstName},</h2>
                                <p style=""margin-top:30px;"">In order to create your account we need your password,so please click the below link to create password:</p>
                                <p><a href=""{resetLink}"">Create Password</a></p> 
                                <p>If you didn't request an account creation then please ignore this mail.</p>
                                </body>
                                </html>";
                if (aspnetUser != null)
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("hallodoc@gmail.com"),
                        Subject = "Create Password for HalloDoc account",
                        Body = message,
                        IsBodyHtml = true
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
            if (model.Symptoms != null)
            {
                requestClient.Notes = model.Symptoms;
            }

            requestClient.Email = model.Email;
            requestClient.IntDate = model.DOB.Day;
            requestClient.StrMonth = model.DOB.Month.ToString();
            requestClient.IntYear = model.DOB.Year;
            requestClient.Street = model.Street;
            requestClient.City = model.City;
            requestClient.State = model.State;
            requestClient.ZipCode = model.FZipCode;
            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

            int requests = _context.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();
            string ConfirmationNumber = string.Concat(region.Abbreviation, model.FirstName.Substring(0, 2).ToUpper(), model.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));
            request.RequestTypeId = 2;
            var id = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (!userExists)
            {
                request.UserId = user.UserId;
            }
            else
            {
                request.UserId = id.UserId;
            }
            request.FirstName = model.FFirstName;
            request.LastName = model.FLastName;
            request.Email = model.FEmail;
            request.PhoneNumber = model.FPhoneNumber;
            request.Status = 1;
            request.CreatedDate = DateTime.Now;
            request.RequestClientId = requestClient.RequestClientId;
            request.ConfirmationNumber = ConfirmationNumber;
            _context.Requests.Add(request);
            _context.SaveChanges();

            if (model.File != null)
            {
                requestWiseFile.RequestId = request.RequestId;
                requestWiseFile.FileName = model.File.FileName;
                requestWiseFile.CreatedDate = DateTime.Now;
                _context.RequestWiseFiles.Add(requestWiseFile);
                _context.SaveChanges();
            }

            requestStatusLog.RequestId = request.RequestId;
            requestStatusLog.Status = 1;
            requestStatusLog.Notes = model.Symptoms;
            requestStatusLog.CreatedDate = DateTime.Now;
            _context.RequestStatusLogs.Add(requestStatusLog);
            _context.SaveChanges();
            return 2;
        }
        public AspNetUser patientCheck(string email)
        {
            return _context.AspNetUsers.FirstOrDefault(u => u.Email == email);
        }


    }
}
