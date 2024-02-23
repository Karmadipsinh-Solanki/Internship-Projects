using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Patient_Interface.PatientRequest
{
    public interface ICreateConciergeRequest
    {
        public BlockRequest EmailFromBlockReq(RequestViewModelConcierge model);
    }
}
