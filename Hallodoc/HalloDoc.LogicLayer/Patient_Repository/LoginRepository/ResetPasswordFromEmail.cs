using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hallodoc.Data;
using HalloDoc.LogicLayer.Patient_Interface.LoginControllerInterface;

namespace HalloDoc.LogicLayer.Patient_Repository.LoginRepository
{
    public class ResetPasswordFromEmail : IResetPasswordFromEmail
    {
        private readonly ApplicationDbContext _db;

        public ResetPasswordFromEmail(ApplicationDbContext db)
        {
            _db = db;
        }
        public AspNetUser ResetPwdFromEmail(CreateNewPassword model)
        {
            return _db.AspNetUsers.FirstOrDefault(u => u.Email == model.email);
        }
    }
}
