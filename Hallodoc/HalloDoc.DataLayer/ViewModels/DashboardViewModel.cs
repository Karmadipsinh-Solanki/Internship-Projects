using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hallodoc.Models.Models;

namespace HalloDoc.DataLayer.ViewModels;

public class DashboardViewModel
{
    public string name { get; set; }

    public List<TableContent> requests { get; set; }

    public partialViewModel partialViewModel { get; set; }

}