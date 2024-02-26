﻿using Hallodoc.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HalloDoc.Models;
public class AdminDashboardTableView
{
    public int new_count { get; set; }
    public int pending_count { get; set; }
    public int active_count { get; set; }
    public int conclude_count { get; set; }
    public int toclose_count { get; set; }
    public int unpaid_count { get; set; }
    //public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
    public List<Request> requests { get; set; } = new List<Request>();
    public IQueryable<Request> query_requests { get; set; }
    public List<Region> regions { get; set; } = new List<Region>();
    public string status { get; set; }
}
