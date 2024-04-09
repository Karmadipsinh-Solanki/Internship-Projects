using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HalloDoc.DataLayer.ViewModels;

public class ForgotPassword
{
    [Required(ErrorMessage = "Please enter the Email")]
    public string email { get; set; }

}