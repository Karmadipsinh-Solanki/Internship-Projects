using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Patient_Interface.PatientDashboardInterface
{
    public interface IMeModalSubmit
    {
        public Region StatesFromRegion(MeViewModel model);
        //public Request CountOfReqAtDate(int count);
        //public Region CurrentUserFromUser(int id);


    }
}
