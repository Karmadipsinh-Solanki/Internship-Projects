//using Microsoft.AspNetCore.Mvc;
//using HalloDoc.Models;
//using System.Diagnostics;
//using Microsoft.EntityFrameworkCore;
//using Npgsql;
//using System.Data;
//using HalloDoc.Data;

using Hallodoc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Hallodoc.Models.Models;
using HalloDoc.DataLayer.ViewModels;
using System.Diagnostics.Metrics;
using HalloDoc.DataLayer.Models;
using HalloDoc.Repository.Auth;
using DocumentFormat.OpenXml.InkML;
using HalloDoc.LogicLayer.Repository;
using HalloDoc.Repository.Interface;
using HalloDoc.LogicLayer.Interface;

namespace Hallodoc.Controllers
{
    [CustomAuthorize("Patient")]
    public class PatientDashboardController : Controller
    {
        private readonly ILogger<PatientDashboardController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IPatient _patient;
        private readonly IJwtService _jwtService;

        public PatientDashboardController(ILogger<PatientDashboardController> logger, ApplicationDbContext context, IPatient patient, IJwtService jwtService)
        {
            _logger = logger;
            _context = context;
            _patient = patient;
            _jwtService = jwtService;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult patientDashboard()
        {
            DashboardViewModel dashboardViewModel = _patient.patientDashboard();
            return View(dashboardViewModel);
        }
       
        public IActionResult viewDoc(int id)
        {
            ViewDocumentModel viewDocumentModel = _patient.viewDoc(id);
            return View(viewDocumentModel);
        }
        [HttpPost]
        public IActionResult viewDoc(ViewDocumentModel model, int id)
        {
            bool check = _patient.viewDoc(model);
            if (check)
            {
                TempData["success"] = "File Uploaded successfully";
            }
            else
            {
                TempData["error"] = "File upload error";
            }
            return RedirectToAction("ViewDoc", new {id = model.RequestId });
        }
        public IActionResult meModal()
        {
            MeViewModel meViewModel = _patient.meModal();
            return View(meViewModel);
        }
        [HttpPost]
        public IActionResult MeModalSubmit(MeViewModel model)
        {
            int check = _patient.meModalSubmit(model);
            if (check == 0)
            {
                ModelState.AddModelError("State", "Currently we are not serving in this region");
                return View(model);
            }
            else if (check == 1)
            {
                ModelState.AddModelError("Email", "This patient is blocked.");
                return View(model);
            }
            else
            {
                return RedirectToAction("PatientDashboard");
            }
        }

        public IActionResult someoneModal()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RelativeModalSubmit(SomeoneElseViewModel model)
        {
            int check = _patient.RelativeModalSubmit(model);
            if (check == 0)
            {
                ModelState.AddModelError("State", "Currently we are not serving in this region");
                return View(model);
            }
            else if (check == 1)
            {
                ModelState.AddModelError("Email", "This patient is blocked.");
                return View(model);
            }
            else
            {
                return RedirectToAction("PatientDashboard");
            }
        }

        public IActionResult profile()
        {
            EditProfileViewModel editProfileViewModel = _patient.profile();
            return View(editProfileViewModel);
        }


        [HttpPost]
        public IActionResult profile(EditProfileViewModel model)
        {
            bool check = _patient.profile(model);
            if (check)
            {
                TempData["success"] = "Profile updated successfully!";
            }
            else
            {
                TempData["error"] = "Profile is not updated!";
            }
            return RedirectToAction("Profile");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
