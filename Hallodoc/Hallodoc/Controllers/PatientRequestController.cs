using Hallodoc.Data;
using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;

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
        private readonly ILogger<LoginController> _logger;
        private readonly ApplicationDbContext _db;

        public PatientRequestController(ILogger<PatientRequestController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult patientLogin(PatientReqViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Request.FirstOrDefault(u => u.FirstName == model.FirstName);
                if (user != null)
                {
                    if (model.PasswordHash == user.PasswordHash)
                    {
                        return RedirectToAction("patientDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("PasswordHash", "Incorrect Password");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Incorrect Email");
                }
            }

            // If we reach here, something went wrong, return the same view with validation errors
            return View();
        }
    }
}
