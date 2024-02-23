using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Patient_Interface.LoginControllerInterface
{
    public interface IPatientLogin
    {
        public AspNetUser ValidateUser(LoginViewModel model);
        public User CheckUserTable(LoginViewModel model);
        public User ValidateUsers(LoginViewModel model);

    }
}
