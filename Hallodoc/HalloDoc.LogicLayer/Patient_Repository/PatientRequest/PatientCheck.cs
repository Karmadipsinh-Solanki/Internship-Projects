using Hallodoc.Data;
using HalloDoc.DataLayer.ViewModels;
using HalloDoc.LogicLayer.Patient_Interface.LoginControllerInterface;
using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc.LogicLayer.Patient_Interface.PatientRequest;

namespace HalloDoc.LogicLayer.Patient_Repository.PatientRequest
{
    public class PatientCheck : IPatientCheck
    {
        private readonly ApplicationDbContext _db;

        public PatientCheck(ApplicationDbContext db)
        {
            _db = db;
        }

        public AspNetUser EmailFromAspnetuserInPatientCheck(string email)
        {
            return _db.AspNetUsers.SingleOrDefault(u => u.Email == email);
        }
    }
}
