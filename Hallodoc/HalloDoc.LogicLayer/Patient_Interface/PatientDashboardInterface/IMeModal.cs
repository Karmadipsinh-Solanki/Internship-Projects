using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Patient_Interface.PatientDashboardInterface
{
    public interface IMeModal
    {
        public User UserIdFromUserInMeModal(int user);
    }
}
