using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HalloDoc.DataLayer.ViewModels;

public class CreateNewPassword
{
    public string password { get; set; }
    public string email { get; set; }
    public DateOnly Date { get; set; }
    public string token { get; set; }

}