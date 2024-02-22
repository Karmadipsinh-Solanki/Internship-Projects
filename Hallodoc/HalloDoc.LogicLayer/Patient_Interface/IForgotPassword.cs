using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.LogicLayer.Patient_Interface
{
    public interface IForgotPassword
    {
        public AspNetUser ForgotpwdAspnetuserEmail(ForgotPassword model);
        public AspNetUser EmailInForgotPwd(ForgotPassword model);
        public Users ForgotpwdUsersEmail(ForgotPassword model);
    }
}
