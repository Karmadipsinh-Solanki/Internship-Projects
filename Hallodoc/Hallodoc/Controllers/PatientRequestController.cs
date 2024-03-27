using Hallodoc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Globalization;
using Hallodoc.Models.Models;
using HalloDoc.DataLayer.ViewModels;
using HalloDoc.DataLayer.Models;
using System.Net.Mail;
using System.Net;
using System.Drawing;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Identity;
using HalloDoc.LogicLayer.Interface;
using HalloDoc.LogicLayer.Repository;
namespace Hallodoc.Controllers
{
    public class PatientRequestController : Controller
    {

        private readonly ILogger<PatientRequestController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ICreateReq _createReq;
        public PatientRequestController(ILogger<PatientRequestController> logger, ApplicationDbContext context, ICreateReq createReq)
        {
            _logger = logger;
            _context = context;
            _createReq = createReq;
        }

        public IActionResult CreatePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult createPassword(CreatePasswordViewModel model)
        {
            bool check = _createReq.createPassword(model);
            if (check)
            {
                TempData["success"] = "Password created successfully!";
                return RedirectToAction("Login","Login");
            }
            else
            {
                TempData["error"] = "User doesnot exists!";
                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult createPatientRequest(RequestViewModelPatient model)
        {
            int check = _createReq.createPatientRequest(model);
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
                TempData["success"] = "Request created successfully!";
                return RedirectToAction("Login","Login");
            }
        }
        [HttpPost]
        public IActionResult createConceirgeRequest(RequestViewModelConcierge model)
        {
            int check = _createReq.createConceirgeRequest(model);
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
                TempData["success"] = "Request created successfully!";
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult createBusinessRequest(RequestViewModelBusiness model)
        {
            int check = _createReq.createBusinessRequest(model);
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
                TempData["success"] = "Request created successfully!";
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public IActionResult createFamilyRequest(RequestViewModelFamily model)
        {
            int check = _createReq.createFamilyRequest(model);
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
                TempData["success"] = "Request created successfully!";
                return RedirectToAction("Login");
            }
        }

        public IActionResult PatientCheck(string email)
        {
            if (email == null)
            {
                return View();
            }

            var existingUser = _context.AspNetUsers.SingleOrDefault(u => u.Email == email);
            //var existingUser = _patientCheck.EmailFromAspnetuserInPatientCheck(email);
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

