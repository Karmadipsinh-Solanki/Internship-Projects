using Hallodoc.Data;
using HalloDoc.DataLayer.ViewModels;
using HalloDoc.LogicLayer.Patient_Interface.LoginControllerInterface;
using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc.LogicLayer.Patient_Interface.PatientDashboardInterface;

namespace HalloDoc.LogicLayer.Patient_Repository.PatientDashboardRepository
{
    public class Profile : IProfile
    {
        private readonly ApplicationDbContext _db;

        public Profile(ApplicationDbContext db)
        {
            _db = db;
        }

        public User UserIdFromUserInProfile(int user)
        {
            return _db.Users.FirstOrDefault(u => u.UserId == user);
        }
        public Region StateFromRegionInProfile(EditProfileViewModel model)
        {
            return _db.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));
        }
        public AspNetUser EmailFromAspnetuserinProfile(EditProfileViewModel model)
        {
            return _db.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
        }
        
    }
}
