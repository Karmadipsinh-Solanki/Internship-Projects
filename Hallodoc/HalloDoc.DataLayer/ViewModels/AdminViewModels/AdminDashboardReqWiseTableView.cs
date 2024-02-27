using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HalloDoc.Models;
public class AdminDashboardReqWiseTableView
{
    public int patient_count { get; set; }
    public int family_count { get; set; }
    public int concierge_count { get; set; }
    public int business_count { get; set; }
    //public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
    public List<Request> requests { get; set; } = new List<Request>();
    public IQueryable<Request> query_requests { get; set; }
    public List<Region> regions { get; set; } = new List<Region>();
    public string status { get; set; }
}
