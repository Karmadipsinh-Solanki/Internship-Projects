using Hallodoc.Data;
using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Models.Models;
using HalloDoc.LogicLayer.Patient_Interface.PatientDashboardInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Patient_Repository.PatientDashboardRepository
{
        public class RelativeModalSubmit : IRelativeModalSubmit
        {
            private readonly ApplicationDbContext _db;

            public RelativeModalSubmit(ApplicationDbContext db)
            {
                _db = db;
            }
            public Region StatesFromRegionInSomeoneModal(SomeoneElseViewModel model)
            {
                return _db.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));
            }
            public User CurrentUserFromUser(int id)
            {
                return _db.Users.FirstOrDefault(u => u.UserId == id);
            }
    }
}

