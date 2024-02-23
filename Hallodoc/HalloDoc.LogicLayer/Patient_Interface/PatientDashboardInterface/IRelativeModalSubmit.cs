using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Patient_Interface.PatientDashboardInterface
{
    public interface IRelativeModalSubmit
    {
        public Region StatesFromRegionInSomeoneModal(SomeoneElseViewModel model);
        public User CurrentUserFromUser(int id);
    }
}
