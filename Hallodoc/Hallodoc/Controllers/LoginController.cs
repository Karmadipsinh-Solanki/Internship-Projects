using Hallodoc.Data;
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
using HalloDoc.LogicLayer.Patient_Interface.LoginControllerInterface;
using HalloDoc.LogicLayer.Patient_Interface.LoginInterface;

namespace Hallodoc.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IPatientLogin _patientLogin;
        private readonly IResetPasswordFromEmail _resetPasswordFromEmail;
        private readonly IForgotPwd _forgotPassword;

        public LoginController(ILogger<LoginController> logger, ApplicationDbContext db, IPatientLogin patientLogin,IResetPasswordFromEmail resetPasswordFromEmail, IForgotPwd forgotPassword)
        {
            _logger = logger;
            _db = db;
            _patientLogin = patientLogin;
            _resetPasswordFromEmail = resetPasswordFromEmail;
            _forgotPassword = forgotPassword;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult patientLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _patientLogin.ValidateUser(model);
                if (user != null)
                {
                    //var passwordHasher = new PasswordHasher<AspNetUser>();
                    //var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.PasswordHash);
                    //if (result == PasswordVerificationResult.Success)
                    //{
                    if (model.PasswordHash == user.PasswordHash)
                    {
                        var user2 = _patientLogin.ValidateUsers(model);
                        //User users = user2.ToList().First();
                        HttpContext.Session.SetInt32("id", user2.UserId);
                        HttpContext.Session.SetString("Name", user2.FirstName);
                        HttpContext.Session.SetString("IsLoggedIn", "true");
                        return RedirectToAction("patientDashboard", "PatientDashboard");
                    }


                    //}
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
            var aspnetUser = _resetPasswordFromEmail.ResetPwdFromEmail(model);
            var passwordHasher = new PasswordHasher<AspNetUser>();
            aspnetUser.PasswordHash = passwordHasher.HashPassword(aspnetUser, model.password);
            //aspnetUser.PasswordHash = model.password;

            //Ishan
            _db.AspNetUsers.Update(aspnetUser);
            _db.SaveChanges();
            //Ishan
            return RedirectToAction("PasswordUpdatedSuccessfully");
        }
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPassword model)
        {
            //var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            //{
            //    Credentials = new NetworkCredential("c3f46c2b681459", "d860d68bf5a0db"),
            //    EnableSsl = true
            //};
            string senderEmail = "tatva.dotnet.Karmadipsinhsolanki@outlook.com";
            string senderPassword = "Karmadips@2311";

            SmtpClient client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
            string email = model.email;
            var aspnetUser = _forgotPassword.ForgotpwdAspnetuserEmail(model);
            //var userdb = _db.Users.FirstOrDefault(u => u.Email == email);
            var userdb = _forgotPassword.ForgotpwdUsersEmail(model);
            var userFirstName = userdb.FirstName;
            //string date = new DateTime.Now;
            var formatedDate = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            string resetLink = $"https://localhost:44339/Login/ResetPassword?email={email}&date={formatedDate}";
            string message = $@"<html>
                                <body>  
                                <h1>Reset password request</h1>
                                <h2>Hii {userFirstName},</h2>
                                <p style=""margin-top:30px;"">To reset your password, click the below link:</p>
                                <p><a href=""{resetLink}"">Reset Password</a></p> 
                                <p>If you didn't request a password reset, you can ignore this email.</p>
                                </body>
                                </html>";
            if (aspnetUser != null)
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "HalloDoc"),
                    Subject = "Reset Password for HalloDoc account",
                    IsBodyHtml = true,
                    Body = message,
                };
                mailMessage.To.Add(email);
                client.Send(mailMessage);
                return RedirectToAction("patientLogin");
            }
            return RedirectToAction("ForgotPassword");
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
        public IActionResult submitrequest()
        {
            return View();
        }
        
    }

}
