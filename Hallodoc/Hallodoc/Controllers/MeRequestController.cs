using Microsoft.AspNetCore.Mvc;

namespace Hallodoc.Controllers
{
    public class MeRequestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
