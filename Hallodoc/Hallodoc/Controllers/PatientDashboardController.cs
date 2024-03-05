//using Microsoft.AspNetCore.Mvc;
//using HalloDoc.Models;
//using System.Diagnostics;
//using Microsoft.EntityFrameworkCore;
//using Npgsql;
//using System.Data;
//using HalloDoc.Data;

using Hallodoc.Data;
using Hallodoc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Hallodoc.Models.Models;
using HalloDoc.DataLayer.ViewModels;
using HalloDoc.LogicLayer.Patient_Interface.PatientDashboardInterface;
using HalloDoc.LogicLayer.Patient_Repository.PatientDashboardRepository;
using System.Diagnostics.Metrics;
using HalloDoc.DataLayer.Models;

namespace Hallodoc.Controllers
{
    public class PatientDashboardController : Controller
    {
        private readonly ILogger<PatientDashboardController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IPatientDashboard _patientDashboard;
        private readonly IViewDoc _viewDoc;
        private readonly IMeModalSubmit _meModalSubmit;
        private readonly IMeModal _meModal;
        private readonly IRelativeModalSubmit _relativeModalSubmit;
        private readonly IProfile _profile;
        

        public PatientDashboardController(ILogger<PatientDashboardController> logger, ApplicationDbContext db, IPatientDashboard patientDashboard , IViewDoc viewDoc, IMeModalSubmit meModalSubmit, IMeModal meModal, IRelativeModalSubmit relativeModalSubmit, IProfile profile)
        {
            _logger = logger;
            _db = db;
            _patientDashboard = patientDashboard;
            _viewDoc = viewDoc;
            _meModalSubmit = meModalSubmit;
            _meModal = meModal;
            _profile = profile;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult patientDashboard()
        {
            var id = HttpContext.Session.GetInt32("id");
            //var curr_user = _db.Users.FirstOrDefault(u => u.UserId == id);
            var curr_user = _patientDashboard.CurrentUserIdFromUser((int)id);

            //var data = _db.TableContents.FromSqlRaw($"SELECT * FROM PatientDashboardData({id})").ToList();
            var data = _patientDashboard.FetchDataFromContentTable((int)id);

            DashboardViewModel dashboardViewModel = new DashboardViewModel
            {
                requests = data,
                name = string.Concat(curr_user.FirstName, ' ', curr_user.LastName),
                partialViewModel = new partialViewModel() { patient_name = string.Concat(curr_user.FirstName + ' ' + curr_user.LastName) }
            };


            //return View();
            return View(dashboardViewModel);
        }
       
        public async Task<IActionResult> viewDoc(int id)
        {
            //to save file in wwwroot,that is uploaded by patient
         
            var user_id = HttpContext.Session.GetInt32("id");
            //var request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            var request = _viewDoc.ListOfIncludeAdminPhysicianToReq(id);
            //var documents = _db.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id).ToList();
            var documents = _viewDoc.ListOfIncludeAdminPhysicianToReqwisefile(id);
            //var user = _db.Users.FirstOrDefault(u => u.UserId == user_id);
            var user = _viewDoc.UserIdFromUser((int)user_id);
            ViewDocumentModel viewDocumentModel = new ViewDocumentModel()
            {
                patient_name = string.Concat(request.RequestClient.FirstName, ' ', request.RequestClient.LastName),
                name = string.Concat(user.FirstName, ' ', user.LastName),
                confirmation_number = request.ConfirmationNumber,
                requestWiseFiles = documents,
                uploader_name = string.Concat(request.FirstName, ' ', request.LastName),
                RequestId = id,
                partialViewModel = new partialViewModel() { patient_name = string.Concat(user.FirstName + ' ' + user.LastName) },
            };
            return View(viewDocumentModel);
        }
        [HttpPost]
        public async Task<IActionResult> viewDoc(ViewDocumentModel model,int id)
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
                requestWiseFile.FileName = model.File.FileName;
                requestWiseFile.CreatedDate = DateTime.Now;
                //ishan
                _db.RequestWiseFiles.Add(requestWiseFile);
                await _db.SaveChangesAsync();
                //Ishan
                return RedirectToAction("viewDoc",new {id=id});
            }
            else
            {
                return RedirectToAction("viewDoc", new { id = id });
            }
        }
        public IActionResult meModal()
        {
            
            var user = HttpContext.Session.GetInt32("id");
            //var userDetail = _db.Users.FirstOrDefault(u => u.UserId == user);
            var userDetail = _meModal.UserIdFromUserInMeModal((int)user);
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

            return View(meViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> MeModalSubmit(MeViewModel model)
        {
            //if (ModelState.IsValid)
            //{

            RequestClient requestClient = new RequestClient();
            Request request = new Request();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();

            //to add one more state,that is to show that we dont give service in particular region
            //var region = _db.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));
            var region = _meModalSubmit.StatesFromRegion(model);
            if (region == null)
            {
                ModelState.AddModelError("State", "Currently we are not serving in this region");
                return View(model);
            }
            ////for block request
            //var blockedUser = _db.BlockRequests.FirstOrDefault(u => u.Email == model.Email);
            //if (blockedUser != null)
            //{
            //    ModelState.AddModelError("Email", "This patient is blocked.");
            //    return View(model);
            //}
            //to save file in wwwroot,that is uploaded by patient
            if (model.File != null && model.File.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", model.File.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.File.CopyToAsync(stream);
                }
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

            //int requests = _meModalSubmit.CountOfReqAtDate(count);
            int requests = _db.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();
            string ConfirmationNumber = string.Concat(region.Abbreviation, model.FirstName.Substring(0, 2).ToUpper(), model.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));
            //
            var id = HttpContext.Session.GetInt32("id");
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
            await _db.SaveChangesAsync();//ishan

            return RedirectToAction("patientDashboard", "PatientDashboard");
        }

        public IActionResult someoneModal()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RelativeModalSubmit(SomeoneElseViewModel model)
        {
            //if (ModelState.IsValid)
            //{

            RequestClient requestClient = new RequestClient();
            Request request = new Request();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();

            //to add one more state,that is to show that we dont give service in particular region
            //var region = _db.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));
            var region = _relativeModalSubmit.StatesFromRegionInSomeoneModal(model);
            if (region == null)
            {
                ModelState.AddModelError("State", "Currently we are not serving in this region");
                return View(model);
            }
            ////for block request
            //var blockedUser = _db.BlockRequests.FirstOrDefault(u => u.Email == model.Email);
            //if (blockedUser != null)
            //{
            //    ModelState.AddModelError("Email", "This patient is blocked.");
            //    return View(model);
            //}
            //to save file in wwwroot,that is uploaded by patient
            if (model.File != null && model.File.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", model.File.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.File.CopyToAsync(stream);
                }
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
            //int requests = _meModalSubmit.CountOfReqAtDate(count);
            int requests = _db.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();
            string ConfirmationNumber = string.Concat(region.Abbreviation, model.FirstName.Substring(0, 2).ToUpper(), model.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));
            //
            var id = HttpContext.Session.GetInt32("id");
            //var curr_user = _db.Users.FirstOrDefault(u => u.UserId == id);
            var curr_user = _relativeModalSubmit.CurrentUserFromUser((int)id);

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

            return RedirectToAction("patientDashboard", "PatientDashboard");
        }

        public IActionResult profile()
        {
            var user = HttpContext.Session.GetInt32("id");
            //var userDetail = _db.Users.FirstOrDefault(u => u.UserId == user);
            var userDetail = _profile.UserIdFromUserInProfile((int)user);
            EditProfileViewModel profileViewModel = new EditProfileViewModel()
            {
                City = userDetail.City,
                Street = userDetail.Street,
                State = userDetail.State,
                DOB = DateTime.Parse(userDetail.IntYear.ToString() + '-' + userDetail.StrMonth + '-' + userDetail.IntDate.ToString()),
                ZipCode = userDetail.ZipCode,
                FirstName = userDetail.FirstName,
                LastName = userDetail.LastName,
                Email = userDetail.Email,
                //meViewModel.DOB = new DateTime(userDetail.IntYear, DateTime.ParseExact(userDetail.StrMonth, "MMMM", CultureInfo.CurrentCulture).Month, userDetail.IntDate);
                PhoneNumber = userDetail.Mobile,
                partialViewModel = new partialViewModel() { patient_name = string.Concat(userDetail.FirstName + ' '+ userDetail) },

            };


            return View(profileViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> profile(EditProfileViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            AspNetUser aspNetUser = new AspNetUser();
            User user = new User();
            RequestClient requestClient = new RequestClient();
            Request request = new Request();
            RequestWiseFile requestWiseFile = new RequestWiseFile();
            RequestStatusLog requestStatusLog = new RequestStatusLog();

            //to add one more state,that is to show that we dont give service in particular region
            //var region = _db.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));
            var region = _profile.StateFromRegionInProfile(model);
            if (region == null)
            {
                ModelState.AddModelError("State", "Currently we are not serving in this region");
                return View(model);
            }

            //var existingUser = _db.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
            var existingUser = _profile.EmailFromAspnetuserinProfile(model);
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
            //if (model.Symptoms != null)
            //{
            //    requestClient.Notes = model.Symptoms;
            //}
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
            var id = HttpContext.Session.GetInt32("id");
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
            //ishan
            _db.Requests.Add(request);
            await _db.SaveChangesAsync();
            //ishan

            //if (model.File != null)
            //{
            //    requestWiseFile.RequestId = request.RequestId;
            //    //IformFile has a property that to store filepath u need to add .filename behind it to store path
            //    requestWiseFile.FileName = model.File.FileName;
            //    requestWiseFile.CreatedDate = DateTime.Now;
            //    _db.RequestWiseFiles.Add(requestWiseFile);
            //    await _db.SaveChangesAsync();
            //}

            requestStatusLog.RequestId = request.RequestId;
            requestStatusLog.Status = 1;
            //requestStatusLog.Notes = model.Symptoms;
            requestStatusLog.CreatedDate = DateTime.Now;
            //ishan
            _db.RequestStatusLogs.Add(requestStatusLog);
            await _db.SaveChangesAsync();
            //ishan

            return RedirectToAction("patientDashboard", "PatientDashboard");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
