using Hallodoc.Data;
using Hallodoc.Models;
using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hallodoc.Controllers
{
    public class PatientRequestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult createPatientRequest()
        {
            return View();
        }
        public IActionResult createFamilyRequest()
        {
            return View();
        }
        public IActionResult createConciergeRequest()
        {
            return View();
        }
        public IActionResult createBusinessRequest()
        {
            return View();
        }
        private readonly ILogger<PatientRequestController> _logger;
        private readonly ApplicationDbContext _db;

        public PatientRequestController(ILogger<PatientRequestController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> createPatientRequest(RequestViewModel model)
        {
            AspNetUser aspNetUser = new AspNetUser();
            User user = new User();
            RequestClient requestClient = new RequestClient();
            Request request = new Request();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();
            Concierge concierge = new Concierge();
            RequestConcierge requestConcierge = new RequestConcierge();

            var existingUser = _db.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
            bool userExists = true;
            if (existingUser == null)
            {
                userExists = false;
                aspNetUser.UserName = model.Email;
                aspNetUser.Email = model.Email;
                aspNetUser.PhoneNumber = model.PhoneNumber;
                aspNetUser.CreatedDate = DateTime.Now;
                aspNetUser.PasswordHash = model.Password;
                _db.AspNetUsers.Add(aspNetUser);
                await _db.SaveChangesAsync();

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
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }

            requestClient.FirstName = model.FirstName;
            requestClient.LastName = model.LastName;
            requestClient.PhoneNumber = model.PhoneNumber;
            requestClient.Location = model.City;
            requestClient.Address = model.Street;
            requestClient.RegionId = 1;
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
            _db.RequestClients.Add(requestClient);
            await _db.SaveChangesAsync();

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
            request.CreatedDate = DateTime.Now;
            //RequestId dropped from requestClient and requestClientId added in request + foreign key
            request.RequestClientId = requestClient.RequestClientId;
            _db.Requests.Add(request);
            await _db.SaveChangesAsync();

            if (model.File != null)
            {
                requestWiseFile.RequestId = request.RequestId;
                requestWiseFile.FileName = model.File;
                requestWiseFile.CreatedDate = DateTime.Now;
                _db.RequestWiseFiles.Add(requestWiseFile);
                await _db.SaveChangesAsync();
            }

            requestStatusLog.RequestId = request.RequestId;
            requestStatusLog.Status = 1;
            requestStatusLog.Notes = model.Symptoms;
            requestStatusLog.CreatedDate = DateTime.Now;
            _db.RequestStatusLogs.Add(requestStatusLog);
            await _db.SaveChangesAsync();

            return RedirectToAction("patientDashboard", "Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> createFamilyRequest(RequestViewModel model)
        {
            AspNetUser aspNetUser = new AspNetUser();
            User user = new User();
            RequestClient requestClient = new RequestClient();
            Request request = new Request();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();
            Concierge concierge = new Concierge();
            RequestConcierge requestConcierge = new RequestConcierge();

            var existingUser = _db.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
            bool userExists = true;
            if (existingUser == null)
            {
                userExists = false;
                aspNetUser.UserName = model.Email;
                aspNetUser.Email = model.Email;
                aspNetUser.PhoneNumber = model.PhoneNumber;
                aspNetUser.CreatedDate = DateTime.Now;
                aspNetUser.PasswordHash = model.Password;
                _db.AspNetUsers.Add(aspNetUser);
                await _db.SaveChangesAsync();

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
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }

            requestClient.FirstName = model.FirstName;
            requestClient.LastName = model.LastName;
            requestClient.PhoneNumber = model.PhoneNumber;
            requestClient.Location = model.City;
            requestClient.Address = model.Street;
            requestClient.RegionId = 1;
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
            _db.RequestClients.Add(requestClient);
            await _db.SaveChangesAsync();

            request.RequestTypeId = 1;
            if (!userExists)
            {
                request.UserId = user.UserId;
            }
            request.FirstName = model.FFirstName;
            request.LastName = model.FLastName;
            request.Email = model.FEmail;
            request.PhoneNumber = model.FPhoneNumber;
            request.RelationName = model.Relation;
            request.Status = 1;
            request.CreatedDate = DateTime.Now;
            //RequestId dropped from requestClient and requestClientId added in request + foreign key
            request.RequestClientId = requestClient.RequestClientId;
            _db.Requests.Add(request);
            await _db.SaveChangesAsync();

            if (model.File != null)
            {
                requestWiseFile.RequestId = request.RequestId;
                requestWiseFile.FileName = model.File;
                requestWiseFile.CreatedDate = DateTime.Now;
                _db.RequestWiseFiles.Add(requestWiseFile);
                await _db.SaveChangesAsync();
            }

            requestStatusLog.RequestId = request.RequestId;
            requestStatusLog.Status = 1;
            requestStatusLog.Notes = model.Symptoms;
            requestStatusLog.CreatedDate = DateTime.Now;
            _db.RequestStatusLogs.Add(requestStatusLog);
            await _db.SaveChangesAsync();

            return RedirectToAction("patientDashboard", "Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> createConciergeRequest(RequestViewModel model)
        {
            AspNetUser aspNetUser = new AspNetUser();
            User user = new User();
            RequestClient requestClient = new RequestClient();
            Request request = new Request();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();
            Concierge concierge = new Concierge();
            RequestConcierge requestConcierge = new RequestConcierge();

            var existingUser = _db.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
            bool userExists = true;
            if (existingUser == null)
            {
                userExists = false;
                aspNetUser.UserName = model.Email;
                aspNetUser.Email = model.Email;
                aspNetUser.PhoneNumber = model.PhoneNumber;
                aspNetUser.CreatedDate = DateTime.Now;
                aspNetUser.PasswordHash = model.Password;
                _db.AspNetUsers.Add(aspNetUser);
                await _db.SaveChangesAsync();

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
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }

            requestClient.FirstName = model.FirstName;
            requestClient.LastName = model.LastName;
            requestClient.PhoneNumber = model.PhoneNumber;
            requestClient.Location = model.City;
            requestClient.Address = model.Street;
            requestClient.RegionId = 1;
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
            _db.RequestClients.Add(requestClient);
            await _db.SaveChangesAsync();

            request.RequestTypeId = 1;
            if (!userExists)
            {
                request.UserId = user.UserId;
            }
            request.FirstName = model.CFirstName;
            request.LastName = model.CLastName;
            request.PhoneNumber = model.CPhoneNumber;
            request.Email = model.CEmail;
            request.Status = 1;
            request.CreatedDate = DateTime.Now;
            //RequestId dropped from requestClient and requestClientId added in request + foreign key
            request.RequestClientId = requestClient.RequestClientId;
            _db.Requests.Add(request);
            await _db.SaveChangesAsync();

            if (model.File != null)
            {
                requestWiseFile.RequestId = request.RequestId;
                requestWiseFile.FileName = model.File;
                requestWiseFile.CreatedDate = DateTime.Now;
                _db.RequestWiseFiles.Add(requestWiseFile);
                await _db.SaveChangesAsync();
            }

            requestStatusLog.RequestId = request.RequestId;
            requestStatusLog.Status = 1;
            requestStatusLog.Notes = model.Symptoms;
            requestStatusLog.CreatedDate = DateTime.Now;
            _db.RequestStatusLogs.Add(requestStatusLog);
            await _db.SaveChangesAsync();

            concierge.ConciergeName = model.CFirstName + " " + model.CLastName;
            concierge.Street = model.CStreet;
            concierge.City = model.CCity;
            concierge.State = model.CState;
            concierge.ZipCode = model.CZipCode;
            concierge.CreatedDate = DateTime.Now;
            _db.Concierges.Add(concierge);
            await _db.SaveChangesAsync();

            requestConcierge.RequestId = request.RequestId;
            requestConcierge.ConciergeId = concierge.ConciergeId;
            _db.RequestConcierges.Add(requestConcierge);
            await _db.SaveChangesAsync();

            return RedirectToAction("patientDashboard", "Login");
        }
    }
}
