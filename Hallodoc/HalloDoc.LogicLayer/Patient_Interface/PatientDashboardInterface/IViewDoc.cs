using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Patient_Interface.PatientDashboardInterface
{
    public interface IViewDoc
    {
        public Request? ListOfIncludeAdminPhysicianToReq(int requestId);
        public List<RequestWiseFile>? ListOfIncludeAdminPhysicianToReqwisefile(int requestId);
        public User UserIdFromUser(int user_id);


    }
}
