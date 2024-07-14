using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicRecordsApp.Data.Models
{
    public partial class Exam
    {
        public Exam()
        {
            Grades = new HashSet<Grade>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Grade> Grades { get; set; }

        public int? CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public virtual Course? Course { get; set; }
    }
}
