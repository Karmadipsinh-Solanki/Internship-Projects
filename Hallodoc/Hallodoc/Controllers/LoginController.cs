using Hallodoc.Data;
using Hallodoc.Models;
using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Data;
using System.Diagnostics;

namespace Hallodoc.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ApplicationDbContext _db;

        public LoginController(ILogger<LoginController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult patientLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult patientsite()
        {
            return View();
        }
        public IActionResult patientLogin()
        {
            return View();
        }

        //Het
        //@ViewData["error"];
        //public IActionResult OnLoginPressed(string Email, string PasswordHash)
        //{
        //    //    Debug.WriteLine(Email)
        //    ;
        //    //    Debug.WriteLine(PasswordHash);
        //    //    return View("Index");

        //    NpgsqlConnection connection = new NpgsqlConnection("Server=localhost;Database=HalloDoc;User Id=postgres;Password=karmadipsinh;Include Error Detail=True");
        //    string Query = "SELECT * FROM \"AspNetUsers\" where \"Email\"=@Email and \"PasswordHash\"=@PasswordHash";
        //    connection.Open();
        //    NpgsqlCommand command = new NpgsqlCommand(Query, connection);
        //    command.Parameters.AddWithValue("@Email", Email);
        //    command.Parameters.AddWithValue("@PasswordHash", PasswordHash);
        //    NpgsqlDataReader reader = command.ExecuteReader();
        //    DataTable dataTable = new DataTable();
        //    dataTable.Load(reader);
        //    int numRows = dataTable.Rows.Count;
        //    if (numRows > 0)
        //    {
        //        return View("patientDashboard");
        //    }
        //    else
        //    {
        //        ViewData["error"] = "Invalid Id Pass";
        //        return View("patientLogin");
        //    }

        //}
        public IActionResult resetPassword()
        {
            return View();
        }
        public IActionResult submitrequest()
        {
            return View();
        }
        public IActionResult patientDashboard()
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
