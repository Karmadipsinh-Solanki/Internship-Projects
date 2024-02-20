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
using Hallodoc.Models.ViewModels;
using System.Drawing;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Hallodoc.Models.Models;

namespace Hallodoc.Controllers
{
    public class PatientDashboardController : Controller
    {
        private readonly ILogger<PatientDashboardController> _logger;
        private readonly ApplicationDbContext _db;

        public PatientDashboardController(ILogger<PatientDashboardController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
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
            var curr_user = _db.Users.FirstOrDefault(u => u.UserId == id);

            var data = _db.TableContents.FromSqlRaw($"SELECT * FROM PatientDashboardData({id})").ToList();
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
            var request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            var documents = _db.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id).ToList();
            var user = _db.Users.FirstOrDefault(u => u.UserId == user_id);
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
                _db.RequestWiseFiles.Add(requestWiseFile);
                await _db.SaveChangesAsync();
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
            var userDetail = _db.Users.FirstOrDefault(u => u.UserId == user);
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
            var region = _db.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));
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
            _db.RequestClients.Add(requestClient);
            await _db.SaveChangesAsync();

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
            _db.Requests.Add(request);
            await _db.SaveChangesAsync();

            if (model.File != null)
            {
                requestWiseFile.RequestId = request.RequestId;
                //IformFile has a property that to store filepath u need to add .filename behind it to store path
                requestWiseFile.FileName = model.File.FileName;
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
            var region = _db.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));
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
            _db.RequestClients.Add(requestClient);
            await _db.SaveChangesAsync();

            //to generate confirmation number(method is given in srs that how to generate confirmation number
            int requests = _db.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();
            string ConfirmationNumber = string.Concat(region.Abbreviation, model.FirstName.Substring(0, 2).ToUpper(), model.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));
            //
            var id = HttpContext.Session.GetInt32("id");
            var curr_user = _db.Users.FirstOrDefault(u => u.UserId == id);

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
            _db.Requests.Add(request);
            await _db.SaveChangesAsync();

            if (model.File != null)
            {
                requestWiseFile.RequestId = request.RequestId;
                //IformFile has a property that to store filepath u need to add .filename behind it to store path
                requestWiseFile.FileName = model.File.FileName;
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

            return RedirectToAction("patientDashboard", "PatientDashboard");
        }

        public IActionResult profile()
        {
            var user = HttpContext.Session.GetInt32("id");
            var userDetail = _db.Users.FirstOrDefault(u => u.UserId == user);
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
            var region = _db.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));
            if (region == null)
            {
                ModelState.AddModelError("State", "Currently we are not serving in this region");
                return View(model);
            }

            var existingUser = _db.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
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
                //user.PasswordHash = 
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
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
            _db.RequestClients.Add(requestClient);
            await _db.SaveChangesAsync();

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
            _db.Requests.Add(request);
            await _db.SaveChangesAsync();

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
            _db.RequestStatusLogs.Add(requestStatusLog);
            await _db.SaveChangesAsync();

            return RedirectToAction("patientDashboard", "PatientDashboard");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
