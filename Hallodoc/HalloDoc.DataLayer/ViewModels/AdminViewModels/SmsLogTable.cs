using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class SmsLogTable
    {
        public int SmsLogId { get; set; }

        public string? Recipient { get; set; }

        public string? Action { get; set; }

        public int RoleName { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime SentDate { get; set; }
        public string Sent { get; set; }
        public int SentTries { get; set; }
        public string ConfirmationNumber { get; set; }
    }
}
