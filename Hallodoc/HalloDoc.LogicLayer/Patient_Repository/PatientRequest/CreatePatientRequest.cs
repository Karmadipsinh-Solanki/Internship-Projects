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
    public class CreatePatientRequest : ICreatePatientRequest
    {
        private readonly ApplicationDbContext _db;

        public CreatePatientRequest(ApplicationDbContext db)
        {
            _db = db;
        }

        public Region StateFromRegionInCreatePatientRequest(RequestViewModelPatient model)
        {
            return _db.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));
        }
        public BlockRequest EmailFromBlockReq(RequestViewModelPatient model)
        {
            return _db.BlockRequests.FirstOrDefault(u => u.Email == model.Email);
        }
        public AspNetUser EmailFromAspnetuser(RequestViewModelPatient model)
        {
            return _db.AspNetUsers.SingleOrDefault(u => u.Email == model.Email);
        }
    }
}
