using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[+][0-9]*$", ErrorMessage = "Only valid numbers are allowed in this field.")]
        public string? PhoneNumber { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime SentDate { get; set; }
        public string Sent { get; set; }
        public int SentTries { get; set; }
        public string ConfirmationNumber { get; set; }
    }
}
