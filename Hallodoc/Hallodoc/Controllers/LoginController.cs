using Hallodoc.Models;
using Hallodoc.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NuGet.Protocol.Plugins;
using System;
using System.Data;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using HalloDoc.DataLayer.ViewModels;
using HalloDoc.ViewModels;
using HalloDoc.Repository.Interface;
using DocumentFormat.OpenXml.InkML;
using HalloDoc.LogicLayer.Interface;
using HalloDoc.LogicLayer.Repository;
//using HalloDoc.DataLayer.Data;
using HalloDoc.DataLayer.Models;
using HalloDoc.DataLayer.Data;

namespace Hallodoc.Controllers
{


    public class LoginController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ILogin _login;

        public LoginController(ILogger<LoginController> logger, ApplicationDbContext context, ILogin login, IJwtService jwtService)
        {
            _logger = logger;
            _context = context;
            _login = login;
            _jwtService = jwtService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                int check = _login.Login(model);
                if (check == 0)
                {
                    AspNetUser user = _login.getAspNetUser(model.Email);
                    var jwtToken = _jwtService.GenerateJWTAuthetication(user);
                    Response.Cookies.Append("jwt", jwtToken);
                    TempData["success"] = "Logged in successfully!";
                    return RedirectToAction("PatientDashboard", "PatientDashboard");
                }

                else if (check == 1)
                {
                    ModelState.AddModelError("PasswordHash", "Incorrect Password");
                }
                else if (check == 2)
                {
                    AspNetUser user = _login.getAspNetUser(model.Email);
                    var jwtToken = _jwtService.GenerateJWTAuthetication(user);
                    Response.Cookies.Append("jwt", jwtToken);
                    TempData["success"] = "Logged in successfully!";
                    return RedirectToAction("AdminDashboard", "AdminDashboard");
                }
                else if (check == 3)
                {
                    ModelState.AddModelError("PasswordHash", "Incorrect Password");
                }
                else
                {
                    ModelState.AddModelError("Email", "This email is not registered");
                }
            }
            return View();
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }
        public IActionResult ResetPassword(string email, DateTime date)
        {
            if (DateTime.Now.Date.Day - date.Day > 0)
            {
                return RedirectToAction("LinkExpired");
            }
            return View();
        }
        public IActionResult PasswordUpdatedSuccessfully()
        {
            return View();
        }
        public IActionResult LinkExpired()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ResetPasswordFromEmail(CreateNewPassword model)
        {
            bool check = _login.resetPasswordFromEmail(model);
            if (check)
            {
                TempData["success"] = "Password updated successfully!";
            }
            else
            {
                TempData["error"] = "Password not updated!";
            }
            return RedirectToAction("PasswordUpdatedSuccessfully");
        }
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPassword model)
        {
            bool check = _login.ForgotPassword(model);
            if (check)
            {
                TempData["success"] = "Reset password link sent to your mail id!";
                return RedirectToAction("Login");
            }
            else
            {
                TempData["error"] = "You are not registered user!";
                return RedirectToAction("ForgotPassword");
            }
        }
        public IActionResult patientsite()
        {
            return View();
        }
        public IActionResult Login()
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
        //        return View("Login");
        //    }

        //}
        public IActionResult submitrequest()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult ReviewAgreement()
        {
            return View();
        }
        public IActionResult AgreementSuccess()
        {
            return View();
        }
        public IActionResult ReviewAgreementSubmit(string email, int requestId, string reason, int status)
        {
            bool check = _login.reviewAgreementSubmit(email, requestId, reason, status);
            if (check)
            {
                TempData["success"] = "Response submitted successfully!";
            }
            else
            {
                TempData["error"] = "Error, response not submitted!";
            }

            return Json(new { success = "Success" });
            //return RedirectToAction("AgreementSuccess");
        }

    }

}
