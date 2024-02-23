using Hallodoc.Models.Models;
using HalloDoc.DataLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Patient_Interface.PatientDashboardInterface
{
    public interface IProfile
    {
        public User UserIdFromUserInProfile(int user);
        public Region StateFromRegionInProfile(EditProfileViewModel model);
        public AspNetUser EmailFromAspnetuserinProfile(EditProfileViewModel model);
    }
}
