using HalloDoc.Controllers;
using HalloDoc.DataLayer.Data;
using HalloDoc.DataLayer.Models;
using HalloDoc.DataLayer.ViewModels.PhysicianViewModel;
using HalloDoc.LogicLayer.Interface;
using HalloDoc.Models;
using HalloDoc.Repository.Auth;
using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Hallodoc.Controllers
{
    [CustomAuthorize("Physician")]
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DoctorController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDoctor _physician;
        //private readonly IPatient _patient;
        private readonly IJwtService _jwtService;

        public DoctorController(ApplicationDbContext context, IDoctor physician, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _physician = physician;
            //_patient = patient;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        
    }
}
