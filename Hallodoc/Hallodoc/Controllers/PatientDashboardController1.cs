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
            var data = _db.TableContents.FromSqlRaw($"SELECT * FROM PatientDashboardData({id})").ToList();
            var curr_user = _db.Users.FirstOrDefault(u => u.UserId == id);
            DashboardViewModel dashboardViewModel = new DashboardViewModel
            {
                requests = data,
                name = string.Concat(curr_user.FirstName, ' ', curr_user.LastName)
            };
            //return View();
            return View(dashboardViewModel);
        }
        public IActionResult ViewDoc()
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
