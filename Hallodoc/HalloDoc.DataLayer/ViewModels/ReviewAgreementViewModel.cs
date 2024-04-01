using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels
{
    public class ReviewAgreementViewModel
    {
        public string PatientCancellationNotes { get; set; }
        public int RequestId { get; set; }
        public string Email { get; set; }
    }
}
