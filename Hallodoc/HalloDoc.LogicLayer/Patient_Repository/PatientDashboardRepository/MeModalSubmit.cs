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
    public class MeModalSubmit : IMeModalSubmit
    {
        private readonly ApplicationDbContext _db;

        public MeModalSubmit(ApplicationDbContext db)
        {
            _db = db;
        }
        public Region StatesFromRegion(MeViewModel model)
        {
            return _db.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));
        }
        //public Request CountOfReqAtDate(int count)
        //{
        //    return _db.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();
        //}
    }
}
