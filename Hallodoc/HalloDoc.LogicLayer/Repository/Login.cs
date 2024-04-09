using HalloDoc.ViewModels;
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
using System.Collections;
//using HalloDoc.DataLayer.Data;
using HalloDoc.DataLayer.Models;
using HalloDoc.DataLayer.Data;

namespace HalloDoc.LogicLayer.Repository
{
    public class Login : ILogin
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
                    //var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.PasswordHash);
                    //if (result == PasswordVerificationResult.Success)
                    if (user.PasswordHash == model.PasswordHash)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                    //return 0;
                }
                else if (userRole == 3)
                {
                    var passwordHasher = new PasswordHasher<AspNetUser>();
                    //var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.PasswordHash);
                    //if (result == PasswordVerificationResult.Success)
                    if (user.PasswordHash == model.PasswordHash)
                    {
                        return 2;
                    }
                    else
                    {
                        return 3;
                    }
                    //return 2;
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
            string Token = Guid.NewGuid().ToString();
            PasswordReset passwordReset = new PasswordReset();
            passwordReset.Token = Token;
            passwordReset.Email = model.email;
            passwordReset.CreatedDate = DateTime.Now;
            passwordReset.IsUpdated = false;
            _context.PasswordResets.Add(passwordReset);
            _context.SaveChanges();

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

            //var userFirstName = userdb.FirstName;
            //string date = new DateTime.Now;
            var formatedDate = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            string resetLink = $"https://localhost:44339/Login/ResetPassword?token={Token}";
            string message = $@"<html>
                                <body>  
                                <h1>Reset password request</h1>
                                <h2>Hii,</h2>
                               
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
        public bool resetPassword(string token)
        {
            PasswordReset passwordReset = _context.PasswordResets.FirstOrDefault(u => u.Token == token);
            if (DateTime.Now.Date.Day - passwordReset.CreatedDate.Day > 0)
            {
                return false;
            }
            else if (passwordReset.IsUpdated == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool resetPasswordFromEmail(CreateNewPassword model)
        {
            PasswordReset passwordReset = _context.PasswordResets.FirstOrDefault(u => u.Token == model.token);
            var aspnetUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == passwordReset.Email);
            //var aspnetUser = _resetPasswordFromEmail.ResetPwdFromEmail(model);
            if (aspnetUser != null)
            {
                var passwordHasher = new PasswordHasher<AspNetUser>();
                //aspnetUser.PasswordHash = passwordHasher.HashPassword(aspnetUser, model.password);
                aspnetUser.PasswordHash = model.password;

                _context.AspNetUsers.Update(aspnetUser);
                _context.SaveChanges();

                passwordReset.IsUpdated = true;
                _context.PasswordResets.Update(passwordReset);
                _context.SaveChanges();
                //PasswordReset passwordReset = _context.PasswordResets.FirstOrDefault(u=>u.)
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool reviewAgreementSubmit(string email, int requestId, string reason, int status)
        {
            int requestid = requestId;
            BitArray check = new BitArray(1);
            check.Set(0, false);
            if (requestId != null)
            {
                var requestToUpdate = _context.Requests.Where(u => u.RequestId == requestId && u.IsDeleted == check).FirstOrDefault();

                if (requestToUpdate != null)
                {
                    requestToUpdate.ModifiedDate = DateTime.Now;
                    requestToUpdate.Status = (short)status;

                    _context.Requests.Update(requestToUpdate);
                    _context.SaveChanges();
                }
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.RequestId = requestId;
                requestStatusLog.Status = (short)status;
                requestStatusLog.Notes = reason;
                requestStatusLog.CreatedDate = DateTime.Now;
                _context.RequestStatusLogs.Add(requestStatusLog);
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
