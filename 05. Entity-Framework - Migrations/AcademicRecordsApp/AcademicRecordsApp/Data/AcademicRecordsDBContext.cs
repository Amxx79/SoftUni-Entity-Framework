using AcademicRecordsApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademicRecordsApp.Data
{
    public partial class AcademicRecordsDBContext : DbContext
    {
        public AcademicRecordsDBContext()
        {
        }

        public AcademicRecordsDBContext(DbContextOptions<AcademicRecordsDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Exam> Exams { get; set; } = null!;
        public virtual DbSet<Grade> Grades { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=AcademicRecordsDB; Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exam>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100);

                entity.HasOne(e => e.Course)
                .WithMany(x => x.Exams)
                .HasForeignKey(e => e.CourseId);
            });

            modelBuilder.Entity<Grade>(entity =>
            {
                entity.Property(e => e.Value).HasColumnType("decimal(3, 2)");

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.Grades)
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Grades_Exams");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Grades)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Grades_Students");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(e => e.FullName).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);

            modelBuilder.Entity<Course>(e =>
            {
                e.Property(e => e.Name).HasMaxLength(100);
            });

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
