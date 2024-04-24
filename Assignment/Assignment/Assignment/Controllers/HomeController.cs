using Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.ViewModels;
using Repository.Assignment.Interface;
using System.Diagnostics;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStudentRepository _student;

        public HomeController(ILogger<HomeController> logger,IStudentRepository student)
        {
            _logger = logger;
            _student = student;
        }

        public IActionResult SearchRecords( int page = 1, int pageSize = 10)
        {
            StudentViewModel model = _student.searchRecords( page = 1, pageSize = 10);
            return PartialView("StudentTable", model);
        }
        public IActionResult Index()
        {
            StudentViewModel model = _student.studentDetail( );
            return View(model);
        }

        public IActionResult Privacy()
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
