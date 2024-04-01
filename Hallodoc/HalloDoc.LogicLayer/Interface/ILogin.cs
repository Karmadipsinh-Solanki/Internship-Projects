using Hallodoc;
using HalloDoc.DataLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Interface
{
    public interface ILogin
    {
        //public LoginViewModel Login();
        public int Login(LoginViewModel model);
        //public ForgotPassword ForgotPassword();
        public bool ForgotPassword(ForgotPassword model);
        public bool resetPasswordFromEmail(CreateNewPassword model);
        public AspNetUser getAspNetUser(string email);
        public bool reviewAgreementSubmit(string email, int requestId, string reason, int status);

    }
}
