﻿using HalloDoc.Repository.Interface;
using Hallodoc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc.LogicLayer.Interface;
using HalloDoc.DataLayer.ViewModels;
using HalloDoc.ViewModels;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;
using System.Diagnostics;

namespace HalloDoc.LogicLayer.Repository
{
    public class Patient : IPatient
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;

        public Patient(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
        }
        public DashboardViewModel patientDashboard()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            int id = cookieModel.userId;

            var curr_user = _context.Users.FirstOrDefault(u => u.UserId == id);
            //var curr_user = _patientDashboard.CurrentUserIdFromUser((int)id);

            var data = _context.TableContents.FromSqlRaw($"SELECT * FROM PatientDashboardData({id})").ToList();
            //var data = _patientDashboard.FetchDataFromContentTable((int)id);

            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            dashboardViewModel.requests = data;
            dashboardViewModel.name = string.Concat(curr_user.FirstName, ' ', curr_user.LastName);
            dashboardViewModel.partialViewModel = new partialViewModel() { patient_name = string.Concat(curr_user.FirstName + ' ' + curr_user.LastName) };

            return dashboardViewModel;
        }
        public ViewDocumentModel viewDoc(int id)
        {
            //to save file in wwwroot,that is uploaded by patient we use jwt and cookies
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            //int id = cookieModel.userId;

            var request = _context.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            //var request = _viewDoc.ListOfIncludeAdminPhysicianToReq(id);
            var documents = _context.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id).ToList();
            //var documents = _viewDoc.ListOfIncludeAdminPhysicianToReqwisefile(id);
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            //var user = _viewDoc.UserIdFromUser((int)user_id);
            ViewDocumentModel viewDocumentModel = new ViewDocumentModel();
            viewDocumentModel.patient_name = string.Concat(request.RequestClient.FirstName, ' ', request.RequestClient.LastName);
            viewDocumentModel.name = string.Concat(user.FirstName, ' ', user.LastName);
            viewDocumentModel.confirmation_number = request.ConfirmationNumber;
            viewDocumentModel.requestWiseFiles = documents;
            viewDocumentModel.uploader_name = string.Concat(request.FirstName, ' ', request.LastName);
            viewDocumentModel.RequestId = id;
            viewDocumentModel.partialViewModel = new partialViewModel() { patient_name = string.Concat(user.FirstName + ' ' + user.LastName) };
            return viewDocumentModel;
        }
        public bool viewDoc(ViewDocumentModel model)
        {
            if (model.File != null && model.File.Length > 0)
            {
                var extractedFileName = Path.GetFileName(model.File.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", model.File.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    model.File.CopyTo(stream);
                }
                RequestWiseFile requestWiseFile = new RequestWiseFile();
                requestWiseFile.RequestId = (int)model.RequestId;
                //IformFile has a property that to store filepath u need to add .filename behind it to store path
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
        public int meModalSubmit(MeViewModel model)
        {
            RequestClient requestClient = new RequestClient();
            Request request = new Request();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();

            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            int id = cookieModel.userId;
            //to add one more state,that is to show that we dont give service in particular region
            var region = _context.Regions.FirstOrDefault(u => u.Name.Trim().ToLower().Replace(" ", "") == model.State.Trim().ToLower().Replace(" ", ""));
            //var region = _meModalSubmit.StatesFromRegion(model);
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
            //decimal lat = decimal.Parse(model.Latitude);
            //decimal lng = decimal.Parse(model.Longitude);
            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

            //to generate confirmation number(method is given in srs that how to generate confirmation number

            //int requests = _meModalSubmit.CountOfReqAtDate(count);
            int requests = _context.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();
            string ConfirmationNumber = string.Concat(region.Abbreviation, model.FirstName.Substring(0, 2).ToUpper(), model.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));
            request.RequestTypeId = 1;

            request.UserId = id;

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
        public MeViewModel meModal()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            int user = cookieModel.userId;
            var userDetail = _context.Users.FirstOrDefault(u => u.UserId == user);
            //var userDetail = _meModal.UserIdFromUserInMeModal((int)user);
            MeViewModel meViewModel = new MeViewModel();
            meViewModel.City = userDetail.City;
            meViewModel.Street = userDetail.Street;
            meViewModel.State = userDetail.State;
            //meViewModel.Room = userDetail.City;
            meViewModel.ZipCode = userDetail.ZipCode;
            meViewModel.FirstName = userDetail.FirstName;
            meViewModel.LastName = userDetail.LastName;
            meViewModel.Email = userDetail.Email;
            //meViewModel.DOB = new DateTime(userDetail.IntYear, DateTime.ParseExact(userDetail.StrMonth, "MMMM", CultureInfo.CurrentCulture).Month, userDetail.IntDate);
            meViewModel.PhoneNumber = userDetail.Mobile;

            return meViewModel;
        }
        public int RelativeModalSubmit(SomeoneElseViewModel model)
        {
            RequestClient requestClient = new RequestClient();
            Request request = new Request();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();

            //token
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            int id = cookieModel.userId;
            //to add one more state,that is to show that we dont give service in particular region
            var region = _context.Regions.FirstOrDefault(u => u.Name.Trim().ToLower().Replace(" ", "") == model.State.Trim().ToLower().Replace(" ", ""));
            //var region = _relativeModalSubmit.StatesFromRegionInSomeoneModal(model);
            if (region == null)
            {
                return 0;
            }
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
            //int requests = _meModalSubmit.CountOfReqAtDate(count);
            int requests = _context.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();
            string ConfirmationNumber = string.Concat(region.Abbreviation, model.FirstName.Substring(0, 2).ToUpper(), model.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));

            var curr_user = _context.Users.FirstOrDefault(u => u.UserId == id);

            request.RequestTypeId = 1;

            request.UserId = id;

            request.FirstName = curr_user.FirstName;
            request.LastName = curr_user.LastName;
            request.Email = curr_user.Email;
            request.PhoneNumber = curr_user.Mobile;
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

        public EditProfileViewModel profile()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            int id = cookieModel.userId;
            var userDetail = _context.Users.FirstOrDefault(u => u.UserId == id);

            partialViewModel patientName = new partialViewModel();
            //var userDetail = _profile.UserIdFromUserInProfile((int)user);
            EditProfileViewModel profileViewModel = new EditProfileViewModel();
            profileViewModel.City = userDetail.City;
            profileViewModel.Street = userDetail.Street;
            profileViewModel.State = userDetail.State;
            profileViewModel.DOB = DateTime.Parse(userDetail.IntYear.ToString() + '-' + userDetail.StrMonth + '-' + userDetail.IntDate.ToString());
            profileViewModel.ZipCode = userDetail.ZipCode;
            profileViewModel.FirstName = userDetail.FirstName;
            profileViewModel.LastName = userDetail.LastName;
            profileViewModel.Email = userDetail.Email;
            //meViewModel.DOB = new DateTime(userDetail.IntYear, DateTime.ParseExact(userDetail.StrMonth, "MMMM", CultureInfo.CurrentCulture).Month, userDetail.IntDate);
            profileViewModel.PhoneNumber = userDetail.Mobile;
            profileViewModel.partialViewModel = new partialViewModel() { patient_name = string.Concat(userDetail.FirstName + ' ' + userDetail.LastName) };
            return profileViewModel;
        }
        public int profile(EditProfileViewModel model)
        {
            //token
            var request2 = _httpContextAccessor.HttpContext.Request;
            var token = request2.Cookies["jwt"];
            CookieModel cookieModel = _jwtService.getDetails(token);
            int id = cookieModel.userId;


            if (id != null)
            {
                var userToUpdate = _context.Users.Where(u => u.UserId == id).FirstOrDefault();
                if (userToUpdate != null)
                {
                    userToUpdate.FirstName = model.FirstName;
                    userToUpdate.LastName = model.LastName;
                    userToUpdate.IntDate = model.DOB.Day;
                    userToUpdate.IntYear = model.DOB.Year;
                    userToUpdate.StrMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.DOB.Month);
                    //userToUpdate.Email = model.Email;
                    userToUpdate.Mobile = model.PhoneNumber;
                    userToUpdate.Street = model.Street;
                    userToUpdate.City = model.City;
                    userToUpdate.State = model.State;
                    userToUpdate.ZipCode = model.ZipCode;
                    userToUpdate.CreatedBy = userToUpdate.CreatedBy;
                    userToUpdate.ModifiedBy = userToUpdate.CreatedBy;
                    userToUpdate.ModifiedDate = DateTime.Now;

                    _context.Users.Update(userToUpdate);
                    _context.SaveChanges();

                }
                return 0;
            }
            else
            {
                return 1;
            }



            //AspNetUser aspNetUser = new AspNetUser();
            //User user = new User();
            //RequestClient requestClient = new RequestClient();
            //Request request = new Request();
            //RequestWiseFile requestWiseFile = new RequestWiseFile();
            //RequestStatusLog requestStatusLog = new RequestStatusLog();

            ////to add one more state,that is to show that we dont give service in particular region
            //var region = _context.Regions.FirstOrDefault(u => u.Name.Trim().ToLower().Replace(" ", "") == model.State.Trim().ToLower().Replace(" ", ""));
            ////var region = _profile.StateFromRegionInProfile(model);
            //if (region == null)
            //{
            //    return 0;
            //}

            //var existingUser = _context.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
            ////var existingUser = _profile.EmailFromAspnetuserinProfile(model);
            //bool userExists = true;
            //if (existingUser == null)
            //{
            //    userExists = false;
            //    if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName))
            //    {
            //        aspNetUser.UserName = "GuestUser";
            //    }
            //    else
            //    {
            //        aspNetUser.UserName = model.FirstName + " " + model.LastName;
            //    }
            //    aspNetUser.Email = model.Email;
            //    aspNetUser.PhoneNumber = model.PhoneNumber;
            //    aspNetUser.CreatedDate = DateTime.Now;
            //    aspNetUser.PasswordHash = model.PasswordHash;
            //    //aspNetUser.UserName = model.FirstName + " " + model.LastName;
            //    _context.AspNetUsers.Add(aspNetUser);
            //    _context.SaveChanges();

            //    user.AspNetUserId = aspNetUser.Id;
            //    user.FirstName = model.FirstName;
            //    user.LastName = model.LastName;
            //    user.Email = model.Email;
            //    user.Mobile = model.PhoneNumber;
            //    user.Street = model.Street;
            //    user.City = model.City;
            //    user.State = model.State;
            //    user.ZipCode = model.ZipCode;
            //    user.IntDate = model.DOB.Day;
            //    user.StrMonth = model.DOB.Month.ToString();
            //    user.IntYear = model.DOB.Year;
            //    user.CreatedBy = aspNetUser.Id;
            //    user.CreatedDate = DateTime.Now;
            //    _context.Users.Add(user);
            //    _context.SaveChanges();
            //}

            //requestClient.FirstName = model.FirstName;
            //requestClient.LastName = model.LastName;
            //requestClient.PhoneNumber = model.PhoneNumber;
            //requestClient.Location = model.City;
            //requestClient.Address = model.Street;
            //requestClient.RegionId = region.RegionId;
            ////if (model.Symptoms != null)
            ////{
            ////    requestClient.Notes = model.Symptoms;
            ////}
            //requestClient.Email = model.Email;
            //requestClient.IntDate = model.DOB.Day;
            //requestClient.StrMonth = model.DOB.Month.ToString();
            //requestClient.IntYear = model.DOB.Year;
            //requestClient.Street = model.Street;
            //requestClient.City = model.City;
            //requestClient.State = model.State;
            //requestClient.ZipCode = model.ZipCode;
            //_context.RequestClients.Add(requestClient);
            //_context.SaveChanges();

            ////to generate confirmation number(method is given in srs that how to generate confirmation number
            //int requests = _context.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();
            //string ConfirmationNumber = string.Concat(region.Abbreviation, model.FirstName.Substring(0, 2).ToUpper(), model.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));

            //request.RequestTypeId = 1;

            //request.UserId = id;

            //request.FirstName = model.FirstName;
            //request.LastName = model.LastName;
            //request.Email = model.Email;
            //request.PhoneNumber = model.PhoneNumber;
            //request.Status = 1;
            //request.ConfirmationNumber = ConfirmationNumber;
            //request.CreatedDate = DateTime.Now;
            ////RequestId dropped from requestClient and requestClientId added in request + foreign key
            //request.RequestClientId = requestClient.RequestClientId;
            //_context.Requests.Add(request);
            //_context.SaveChanges();

            ////if (model.File != null)
            ////{
            ////    requestWiseFile.RequestId = request.RequestId;
            ////    //IformFile has a property that to store filepath u need to add .filename behind it to store path
            ////    requestWiseFile.FileName = model.File.FileName;
            ////    requestWiseFile.CreatedDate = DateTime.Now;
            ////    _context.RequestWiseFiles.Add(requestWiseFile);
            ////    await _context.SaveChangesAsync();
            ////}

            //requestStatusLog.RequestId = request.RequestId;
            //requestStatusLog.Status = 1;
            ////requestStatusLog.Notes = model.Symptoms;
            //requestStatusLog.CreatedDate = DateTime.Now;
            //_context.RequestStatusLogs.Add(requestStatusLog);
            //_context.SaveChanges();

            //return 2;
        }

        

        public SomeoneElseViewModel someoneModal()
        {
            throw new NotImplementedException();
        }

        public int relativeModalSubmit(SomeoneElseViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
