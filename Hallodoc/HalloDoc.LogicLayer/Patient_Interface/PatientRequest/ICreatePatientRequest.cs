using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Patient_Interface.PatientRequest
{
    public interface ICreatePatientRequest
    {
        public Region StateFromRegionInCreatePatientRequest(RequestViewModelPatient model);
        public BlockRequest EmailFromBlockReq(RequestViewModelPatient model);
        public AspNetUser EmailFromAspnetuser(RequestViewModelPatient model);
    }

}
