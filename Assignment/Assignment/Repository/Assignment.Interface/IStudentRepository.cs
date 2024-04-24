using Assignment;
using Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Assignment.Interface
{
    public interface IStudentRepository
    {
        public StudentViewModel studentDetail( int page = 1, int pageSize = 10);

        public StudentViewModel searchRecords( int page = 1, int pageSize = 10);
    }
}
