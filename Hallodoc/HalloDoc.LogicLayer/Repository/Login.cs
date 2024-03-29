﻿using HalloDoc.ViewModels;
using Hallodoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc.DataLayer.ViewModels;
using HalloDoc.LogicLayer.Interface;
using HalloDoc.Repository.Interface;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;

namespace HalloDoc.LogicLayer.Repository
{
    public class Login:ILogin
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;

        public Login(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
        }
        int ILogin.Login(LoginViewModel model)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);


            if (user != null)
            {
                var userRole = _context.AspNetUserRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                if (userRole == 2)
                {
                    var passwordHasher = new PasswordHasher<AspNetUser>();
                    var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.PasswordHash);
                    if (result == PasswordVerificationResult.Success)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else if (userRole == 3)
                {
                    var passwordHasher = new PasswordHasher<AspNetUser>();
                    //var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.PasswordHash);
                    //if (result == PasswordVerificationResult.Success)
                    //{
                    //    return 2;
                    //}
                    //else
                    //{
                    //    return 3;
                    //}
                    return 2;
                }
                else
                {
                    return 4;
                }
            }
            else
            {
                return 4;
            }
        }
        public bool ForgotPassword(ForgotPassword model)
        {
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
            var aspnetUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == model.email);
            var userdb = _context.Users.FirstOrDefault(u => u.Email == email);
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
                return true;
            }
            return false;
        }
        public bool resetPasswordFromEmail(CreateNewPassword model)
        {
            var aspnetUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == model.email);
            //var aspnetUser = _resetPasswordFromEmail.ResetPwdFromEmail(model);
            if (aspnetUser != null)
            {
                var passwordHasher = new PasswordHasher<AspNetUser>();
                aspnetUser.PasswordHash = passwordHasher.HashPassword(aspnetUser, model.password);
                //aspnetUser.PasswordHash = model.password;

                _context.AspNetUsers.Update(aspnetUser);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public AspNetUser getAspNetUser(string email)
        {
            AspNetUser aspNetUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == email);
            return aspNetUser;
        }
    }
}
