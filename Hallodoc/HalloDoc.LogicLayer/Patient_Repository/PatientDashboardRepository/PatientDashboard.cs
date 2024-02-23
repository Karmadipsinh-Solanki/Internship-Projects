using Hallodoc.Data;
using HalloDoc.DataLayer.ViewModels;
using Hallodoc.Models.Models;
using HalloDoc.LogicLayer.Patient_Interface.LoginInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc.LogicLayer.Patient_Interface.PatientDashboardInterface;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.LogicLayer.Patient_Repository.PatientDashboardRepository
{
    //public class PatientDashboard : IPatientDashboard
    //{
    //    private readonly ApplicationDbContext _db;

    //    public PatientDashboard(ApplicationDbContext db)
    //    {
    //        _db = db;
    //    }
    //    public AspNetUser ValidateUser(LoginViewModel model)
    //    {
    //        return _db.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);
    //    }
    //}

    public class PatientDashboard : IPatientDashboard
    {
        private readonly ApplicationDbContext _db;

        public PatientDashboard(ApplicationDbContext db)
        {
            _db = db;
        }

        public User CurrentUserIdFromUser(int id)

        {
            // use the user object to retrieve the current user
            // ...

            return _db.Users.FirstOrDefault(u => u.UserId == id);
        }
        public List<TableContent> FetchDataFromContentTable(int id)
        {
            return _db.TableContents.FromSqlRaw($"SELECT * FROM PatientDashboardData({id})").ToList();
        }
        

    }
}
