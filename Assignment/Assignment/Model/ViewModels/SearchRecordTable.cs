using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModels
{
    public class SearchRecordTable
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? CourseId { get; set; }
        public int? Age { get; set; }
        public string? Email { get; set; }
        public bool Gender { get; set; }
        public string? Course { get; set; }
        public string? Grade { get; set; }
    }
}
