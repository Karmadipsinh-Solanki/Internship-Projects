using Hallodoc.Data;
using Hallodoc.Models.Models;
using HalloDoc.DataLayer.ViewModels;
using HalloDoc.LogicLayer.Patient_Interface.PatientDashboardInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Patient_Repository.PatientDashboardRepository
{
    public class MeModal : IMeModal
    {
        private readonly ApplicationDbContext _db;

        public MeModal(ApplicationDbContext db)
        {
            _db = db;
        }
        public User UserIdFromUserInMeModal(int user)
        {
            return _db.Users.FirstOrDefault(u => u.UserId == user);
        }
    }
}