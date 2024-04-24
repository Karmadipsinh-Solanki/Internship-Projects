using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class BlockHistoryTable
    {
        public int BlockRequestId { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[+][0-9]*$", ErrorMessage = "Only valid numbers are allowed in this field.")]

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public bool IsActive { get; set; }

        public string? Reason { get; set; }

        public int RequestId { get; set; }

        //public string? Ip { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public string FirstName { get; set; }
    }
}
