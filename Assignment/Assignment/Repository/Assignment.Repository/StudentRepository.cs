using Assignment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Model.ViewModels;
using Repository.Assignment.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Assignment.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;
        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public StudentViewModel studentDetail(  int page = 1, int pageSize = 10)
        {
            BitArray check = new BitArray(1);
            check.Set(0, false);

            IQueryable<Student> query = _context.Students;
            //var student = _context.Students.FirstOrDefault(r => r.Id == id);
            //Student student = new Student();
            StudentViewModel studentViewModel = new StudentViewModel();
            //{
            //    Id = student.Id,
            //    FirstName = student.FirstName,
            //    LastName = student.LastName,
            //    Email = student.Email,
            //    Age = student.Age,
            //    //Gender = student.Gender,
            //    Course = student.Course,
            //    Grade = student.Grade
            //};

            List<SearchRecordTable> searchRecordTable = new List<SearchRecordTable>();
            List<Student> students= query.ToList();
            for (int i = 0; i < students.Count; i++)
            {
                int num = i;
                int id = students[i].Id;
                searchRecordTable.Add(new SearchRecordTable
                {

                    Id = students[i].Id,
                    FirstName = students[i].FirstName,
                    LastName = students[i].LastName,
                    Email = students[i].Email,
                    Age = students[i].Age,
                    Gender = students[i].Gender,
                    Course = students[i].Course,
                    Grade = students[i].Grade
                });
            }

            studentViewModel.Query = searchRecordTable.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return studentViewModel;
        }
        public StudentViewModel searchRecords( int page = 1, int pageSize = 10)
        {
            StudentViewModel model = studentDetail( page = 1, pageSize = 10);
            return model;
        }
    }
}
