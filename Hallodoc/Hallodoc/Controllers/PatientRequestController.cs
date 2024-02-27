using Hallodoc.Data;
using Hallodoc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Globalization;
using Hallodoc.Models.Models;
using HalloDoc.DataLayer.ViewModels;
using HalloDoc.LogicLayer.Patient_Repository.PatientRequest;
using HalloDoc.LogicLayer.Patient_Interface.PatientRequest;
using HalloDoc.DataLayer.Models;
namespace Hallodoc.Controllers
{
    public class PatientRequestController : Controller
    {
        
        private readonly ILogger<PatientRequestController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly ICreatePatientRequest _createPatientRequest;
        private readonly ICreateFamilyRequest _createFamilyRequest;
        private readonly ICreateConciergeRequest _createConciergeRequest;
        private readonly ICreateBusinessRequest _createBusinessRequest;
        private readonly IPatientCheck _patientCheck;
        

        public PatientRequestController(ILogger<PatientRequestController> logger, ApplicationDbContext db, ICreatePatientRequest createPatientRequest, ICreateFamilyRequest createFamilyRequest, ICreateConciergeRequest createConciergeRequest, ICreateBusinessRequest createBusinessRequest, IPatientCheck patientCheck)
        {
            _logger = logger;
            _db = db;
            _createPatientRequest = createPatientRequest;
            _createFamilyRequest = createFamilyRequest;
            _createConciergeRequest = createConciergeRequest;
            _createBusinessRequest = createBusinessRequest;
            _patientCheck = patientCheck;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> createPatientRequest(RequestViewModelPatient model)
        {
            //if (ModelState.IsValid)
            //{
                AspNetUser aspNetUser = new AspNetUser();
                User user = new User();
                RequestClient requestClient = new RequestClient();
                Request request = new Request();
                RequestWiseFile requestWiseFile = new RequestWiseFile();
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                Concierge concierge = new Concierge();
                RequestConcierge requestConcierge = new RequestConcierge();

            //to add one more state,that is to show that we dont give service in particular region
            //var region = _db.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));
            var region = _createPatientRequest.StateFromRegionInCreatePatientRequest(model);
            if (region == null)
            {
                ModelState.AddModelError("State", "Currently we are not serving in this region");
                return View(model);
            }
            //for block request
            //var blockedUser = _db.BlockRequests.FirstOrDefault(u => u.Email == model.Email);
            var blockedUser = _createPatientRequest.EmailFromBlockReq(model);
            if (blockedUser != null)
            {
                ModelState.AddModelError("Email", "This patient is blocked.");
                return View(model);
            }
            //to save file in wwwroot,that is uploaded by patient
            if (model.File != null && model.File.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", model.File.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.File.CopyToAsync(stream);
                }
            }

            //var existingUser = _db.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
            var existingUser = _createPatientRequest.EmailFromAspnetuser(model);
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
                    _db.AspNetUsers.Add(aspNetUser);
                    await _db.SaveChangesAsync();
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
                    //ishan
                    _db.Users.Add(user);
                    await _db.SaveChangesAsync();
                    //ishan
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
            //ishan
                _db.RequestClients.Add(requestClient);
                await _db.SaveChangesAsync();
            //ishan

            //to generate confirmation number(method is given in srs that how to generate confirmation number
            int requests = _db.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();
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
                //ishan
                 _db.Requests.Add(request);
                await _db.SaveChangesAsync();
                //ishan 

            if (model.File != null)
                {
                    requestWiseFile.RequestId = request.RequestId;
                //IformFile has a property that to store filepath u need to add .filename behind it to store path
                requestWiseFile.FileName = model.File.FileName;
                    requestWiseFile.CreatedDate = DateTime.Now;
                //ishan
                _db.RequestWiseFiles.Add(requestWiseFile);
                    await _db.SaveChangesAsync();
                //ishan

                }

                requestStatusLog.RequestId = request.RequestId;
                requestStatusLog.Status = 1;
                requestStatusLog.Notes = model.Symptoms;
                requestStatusLog.CreatedDate = DateTime.Now;
            //ishan
            _db.RequestStatusLogs.Add(requestStatusLog);
                await _db.SaveChangesAsync();
            //ishan
            //}
            return RedirectToAction("patientLogin", "Login");
        }







        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> createFamilyRequest(RequestViewModelFamily model)
        {
            if (ModelState.IsValid)
            {
                AspNetUser aspNetUser = new AspNetUser();
                User user = new User();
                RequestClient requestClient = new RequestClient();
                Request request = new Request();
                RequestWiseFile requestWiseFile = new RequestWiseFile();
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                Concierge concierge = new Concierge();
                RequestConcierge requestConcierge = new RequestConcierge();

                //var existingUser = _db.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
                var existingUser = _createFamilyRequest.EmailFromBlockReq(model);
                bool userExists = true;
                if (existingUser == null)
                {
                    userExists = false;
                    aspNetUser.UserName = model.Email;
                    aspNetUser.Email = model.Email;
                    aspNetUser.PhoneNumber = model.PhoneNumber;
                    aspNetUser.CreatedDate = DateTime.Now;
                    aspNetUser.PasswordHash = model.Password;
                    //ishan
                    _db.AspNetUsers.Add(aspNetUser);
                    await _db.SaveChangesAsync();
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
                    //ishan
                    _db.Users.Add(user);
                    await _db.SaveChangesAsync();
                    //ishan
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
                //ishan
                _db.RequestClients.Add(requestClient);
                await _db.SaveChangesAsync();
                //ishan

                request.RequestTypeId = 2;
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
                //ishan
                _db.Requests.Add(request);
                await _db.SaveChangesAsync();
                //ishan

                if (model.File != null)
                {
                    requestWiseFile.RequestId = request.RequestId;
                    //IformFile has a property that to store filepath u need to add .filename behind it to store path
                    requestWiseFile.FileName = model.File.FileName;
                    requestWiseFile.CreatedDate = DateTime.Now;
                    //ishan
                    _db.RequestWiseFiles.Add(requestWiseFile);
                    await _db.SaveChangesAsync();
                    //ishan
                }

                requestStatusLog.RequestId = request.RequestId;
                requestStatusLog.Status = 1;
                requestStatusLog.Notes = model.Symptoms;
                requestStatusLog.CreatedDate = DateTime.Now;
                //ishan
                _db.RequestStatusLogs.Add(requestStatusLog);
                await _db.SaveChangesAsync();
                //ishan
            }
            return RedirectToAction("patientLogin", "Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> createConciergeRequest(RequestViewModelConcierge model)
        {

            AspNetUser aspNetUser = new AspNetUser();
            User user = new User();
            RequestClient requestClient = new RequestClient();
            Request request = new Request();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();
            Concierge concierge = new Concierge();
            RequestConcierge requestConcierge = new RequestConcierge();

            //var existingUser = _db.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
            var existingUser = _createConciergeRequest.EmailFromBlockReq(model);
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

            request.RequestTypeId = 3;
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

            return RedirectToAction("patientLogin", "Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> createBusinessRequest(RequestViewModelBusiness model)
        {

            AspNetUser aspNetUser = new AspNetUser();
            User user = new User();
            RequestClient requestClient = new RequestClient();
            Request request = new Request();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();
            Concierge concierge = new Concierge();
            RequestConcierge requestConcierge = new RequestConcierge();
            Business business = new Business();

            //var existingUser = _db.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
            var existingUser = _createBusinessRequest.EmailFromBlockReq(model);
            bool userExists = true;
            if (existingUser == null)
            {
                userExists = false;
                aspNetUser.UserName = model.Email;
                aspNetUser.Email = model.Email;
                aspNetUser.PhoneNumber = model.PhoneNumber;
                aspNetUser.CreatedDate = DateTime.Now;
                aspNetUser.PasswordHash = model.Password;
                //ishan
                _db.AspNetUsers.Add(aspNetUser);
                await _db.SaveChangesAsync();
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
                //ishan
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                //ishan
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
            //ishan
            _db.RequestClients.Add(requestClient);
            await _db.SaveChangesAsync();
            //ishan

            request.RequestTypeId = 4;
            if (!userExists)
            {
                request.UserId = user.UserId;
            }
            request.FirstName = model.BFirstName;
            request.LastName = model.BLastName;
            request.Email = model.BEmail;
            request.PhoneNumber = model.BPhoneNumber;
            //request.RelationName = model.Relation;
            request.Status = 1;
            request.CreatedDate = DateTime.Now;
            //RequestId dropped from requestClient and requestClientId added in request + foreign key
            request.RequestClientId = requestClient.RequestClientId;
            //ishan
            _db.Requests.Add(request);
            await _db.SaveChangesAsync();
            //ishan

            if (model.File != null)
            {
                requestWiseFile.RequestId = request.RequestId;
                requestWiseFile.FileName = model.File;
                //ishan
                requestWiseFile.CreatedDate = DateTime.Now;
                _db.RequestWiseFiles.Add(requestWiseFile);
                await _db.SaveChangesAsync();
                //ishan
            }

            requestStatusLog.RequestId = request.RequestId;
            requestStatusLog.Status = 1;
            requestStatusLog.Notes = model.Symptoms;
            requestStatusLog.CreatedDate = DateTime.Now;
            //ishan
            _db.RequestStatusLogs.Add(requestStatusLog);
            await _db.SaveChangesAsync();
            //ishan
            //business.Name = model.BFirstName + " " + model.BLastName;


            return RedirectToAction("patientLogin", "Login");
        }
        public IActionResult PatientCheck(string email)
        {
            if (email == null)
            {
                return View();
            }

            //var existingUser = _db.AspNetUsers.SingleOrDefault(u => u.Email == email);
            var existingUser = _patientCheck.EmailFromAspnetuserInPatientCheck(email);
            bool isValidEmail;
            if (existingUser == null)
            {
                isValidEmail = false;
            }
            else
            {
                isValidEmail = true;
            }
            return Json(new { isValid = isValidEmail });
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult createPatientRequest()
        {
            return View();
        }

        public IActionResult createConciergeRequest()
        {
            return View();
        }
        public IActionResult createFamilyRequest()
        {
            return View();
        }
        public IActionResult createBusinessRequest()
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

