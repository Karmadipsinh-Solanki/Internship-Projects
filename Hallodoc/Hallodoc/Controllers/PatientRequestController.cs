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
    }
}
