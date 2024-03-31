using HalloDoc.DataLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Interface
{
    public interface IPatient
    {
        public DashboardViewModel patientDashboard();
        public bool viewDoc(ViewDocumentModel model);
        public ViewDocumentModel viewDoc(int id); 
        public MeViewModel meModal();
        public int meModalSubmit(MeViewModel model);
        public SomeoneElseViewModel someoneModal();
        public int RelativeModalSubmit(SomeoneElseViewModel model);
        public EditProfileViewModel profile();
        public bool profile(EditProfileViewModel model);
    }
}
