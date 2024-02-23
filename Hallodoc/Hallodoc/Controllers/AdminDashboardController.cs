using Microsoft.AspNetCore.Mvc;

namespace Hallodoc.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
