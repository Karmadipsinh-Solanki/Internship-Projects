using HalloDoc.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataLayer.ViewModels.AdminViewModels
{
    public class RequestWithFileCountModel
    {
        public Request Request { get; set; }
        public int FileCount { get; set; }
    }
}
