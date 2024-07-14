using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicRecordsApp.Data.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public ICollection<Student> Students { get; set; } = new HashSet<Student>();
        public ICollection<Exam> Exams { get; set; }
    }
}
