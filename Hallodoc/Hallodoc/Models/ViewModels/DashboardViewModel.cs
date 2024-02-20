using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hallodoc.Models.Models;
namespace Hallodoc.Models.ViewModels;

public class DashboardViewModel
{
    public string name { get; set; }

    public List<TableContent> requests { get; set; }

    public partialViewModel partialViewModel { get; set; }

}