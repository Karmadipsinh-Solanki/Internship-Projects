using Hallodoc.Data;
using Hallodoc.Models.Models;
using HalloDoc.LogicLayer.Patient_Interface.PatientDashboardInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Patient_Repository.PatientDashboardRepository
{
    public class ViewDoc : IViewDoc
    {
        private readonly ApplicationDbContext _db;

        public ViewDoc(ApplicationDbContext db)
        {
            _db = db;
        }
        public Request? ListOfIncludeAdminPhysicianToReq(int requestId)
        {
            //return _db.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            
                return _db.Requests.Include(r => r.RequestClient)
                                   .FirstOrDefault(u => u.RequestId == requestId);
            
        }
        public List<RequestWiseFile>? ListOfIncludeAdminPhysicianToReqwisefile(int id)
        {
            //return _db.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            
                return _db.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id).ToList();
           
        }
        public User UserIdFromUser(int user_id)
        {
            return _db.Users.FirstOrDefault(u => u.UserId == user_id);
        }
    }
}
