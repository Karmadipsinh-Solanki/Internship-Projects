using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.LogicLayer.Patient_Interface.LoginInterface
{
    public interface IForgotPwd
    {
        public AspNetUser ForgotpwdAspnetuserEmail(ForgotPassword model);
        //public AspNetUser EmailInForgotPwd(ForgotPassword model);
        public User ForgotpwdUsersEmail(ForgotPassword model);
    }
}
