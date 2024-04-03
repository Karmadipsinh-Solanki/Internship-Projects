using Hallodoc;
using HalloDoc.DataLayer.Models;
using HalloDoc.DataLayer.ViewModels;
using HalloDoc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Interface
{
    public interface ICreateReq
    {
        public bool createPassword(CreatePasswordViewModel model);
        public int createPatientRequest(RequestViewModelPatient model);
        public int createConciergeRequest(RequestViewModelConcierge model);
        public int createBusinessRequest(RequestViewModelBusiness model);
        public int createFamilyRequest(RequestViewModelFamily model);
        public AspNetUser patientCheck(string email);


    }
}
