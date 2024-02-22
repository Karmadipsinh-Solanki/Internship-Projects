using HalloDoc.LogicLayer.Patient_Interface;
using HalloDoc.LogicLayer.Patient_Interface;
using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Data;
using Microsoft.Extensions.Logging;
using Hallodoc.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.LogicLayer.Patient_Repository
{
    public class PatientLogin : IPatientLogin
    {
        private readonly ApplicationDbContext _db;

        public PatientLogin( ApplicationDbContext db)
        {
            _db = db;
        }
        //public AspNetUser ValidateAspNetUser(LoginViewModel model)
        //{
        //    return _context.AspNetUser.FirstOrDefault(u => u.UserName == model.Username);
        //}
        //public AspNetUser ResetPasswordFromEmail(CreateNewPassword model)
        //{
        //    return RedirectToAction("PasswordUpdatedSuccessfully");
        //}

        public AspNetUser ValidateUser(LoginViewModel model)
        {
            return _db.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);
        }

        public User CheckUserTable(LoginViewModel model)
        {
            return (User)_db.Users.Where(x => x.Email == model.Email);
        }
        public User ValidateUsers(LoginViewModel model)
        {
            User user = _db.Users.FirstOrDefault(x => x.Email == model.Email);
            //User user1 = new User { UserId = user.UserId };
            return user;
        }
       
    }
}
