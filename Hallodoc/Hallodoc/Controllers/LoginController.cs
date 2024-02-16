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
                        var user2 = _db.Users.Where(x => x.Email == model.Email);
                        User users = user2.ToList().First();
                        HttpContext.Session.SetInt32("id", users.UserId);
                        HttpContext.Session.SetString("Name", users.FirstName);
                        HttpContext.Session.SetString("IsLoggedIn", "true");
                        return RedirectToAction("patientDashboard", "PatientDashboard");
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
        public IActionResult resetPassword(ResetPasswordViewModel model)
        {
        //    if (ModelState.IsValid)
        //    {
        //        if (user != null)
        //        {
        //            if (model.Username == user.Username)
        //            {
        //                var user2 = _db.Users.Where(x => x.Username == model.Username);
        //                return RedirectToAction("patientDashboard", "PatientDashboard");
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("PasswordHash", "Incorrect Username");
        //            }
        //        }
                
        //    }

            // If we reach here, something went wrong, return the same view with validation errors
            return View();
        }
        public IActionResult submitrequest()
        {
            return View();
        }
        
    }

}
