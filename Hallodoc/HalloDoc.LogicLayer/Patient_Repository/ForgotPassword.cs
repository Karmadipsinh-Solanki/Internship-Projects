using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hallodoc.Data;
using HalloDoc.LogicLayer.Patient_Interface;

namespace HalloDoc.LogicLayer.Patient_Repository
{
    public class ForgotPassword : IForgotPassword
    {
        private readonly ApplicationDbContext _db;

        public ForgotPassword(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public AspNetUser ForgotpwdAspnetuserEmail(ForgotPassword model)
        {
            return _db.AspNetUsers.FirstOrDefault(u => u.Email == email);
        }
        public AspNetUser ForgotpwdUsersEmail(ForgotPassword model)
        {
            return _db.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}
