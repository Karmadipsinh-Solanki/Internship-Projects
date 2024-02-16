//using Microsoft.AspNetCore.Mvc;
//using HalloDoc.Models;
//using System.Diagnostics;
//using Microsoft.EntityFrameworkCore;
//using Npgsql;
//using System.Data;
//using HalloDoc.Data;

using Hallodoc.Data;
using Hallodoc.Models;
using HalloDoc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
                name = string.Concat(curr_user.FirstName, ' ', curr_user.LastName)
            };


            //return View();
            return View(dashboardViewModel);
        }
        public IActionResult ViewDocument(int requestid)
        {
            var user_id = HttpContext.Session.GetInt32("id");
            var request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == requestid);
            var documents = _db.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == requestid).ToList();
            var user = _db.Users.FirstOrDefault(u => u.UserId == user_id);
            ViewDocumentModel viewDocumentModal = new ViewDocumentModel()
            {
                patient_name = string.Concat(request.RequestClient.FirstName, ' ', request.RequestClient.LastName),
                name = string.Concat(user.FirstName, ' ', user.LastName),
                confirmation_number = request.ConfirmationNumber,
                requestWiseFiles = documents,
                uploader_name = string.Concat(request.FirstName, ' ', request.LastName),
                RequestId = requestid,
            };
            return View(viewDocumentModal);
        }
        public IActionResult meModal()
        {
            return View();
        }
        public IActionResult someoneModal()
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
