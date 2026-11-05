
using Microsoft.EntityFrameworkCore;
using UMS.Application.Interfaces;
using UMS.Domain.Entities;

namespace UMS.Infrastructure.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Semester> Semesters { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<DropCourseRequest> DropCourseRequests { get; set; }

        public DbSet<Timetable> Timetables { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<SectionTeacher> SectionTeachers { get; set; }

        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- Unique constraints ----------

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Department>()
                .HasIndex(d => d.Code)
                .IsUnique();

            modelBuilder.Entity<Course>()
                .HasIndex(c => c.Code)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentNumber)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.EmployeeNumber)
                .IsUnique();

            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.Email)
                .IsUnique();

            modelBuilder.Entity<Section>()
                .HasIndex(s => new { s.CourseId, s.SemesterId, s.Name })
                .IsUnique();

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.SectionId })
                .IsUnique();

            // ---------- Composite primary key ----------

            modelBuilder.Entity<SectionTeacher>()
                .HasKey(st => new { st.SectionId, st.TeacherId });

            // ---------- Student relationships ----------

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Department)
                .WithMany()
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Teacher relationships ----------

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Department)
                .WithMany()
                .HasForeignKey(t => t.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Course relationships ----------

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany()
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Section relationships ----------

            modelBuilder.Entity<Section>()
                .HasOne(s => s.Course)
                .WithMany()
                .HasForeignKey(s => s.CourseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Section>()
                .HasOne(s => s.Semester)
                .WithMany()
                .HasForeignKey(s => s.SemesterId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- SectionTeacher relationships ----------

            modelBuilder.Entity<SectionTeacher>()
                .HasOne(st => st.Section)
                .WithMany()
                .HasForeignKey(st => st.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SectionTeacher>()
                .HasOne(st => st.Teacher)
                .WithMany()
                .HasForeignKey(st => st.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Enrollment relationships ----------

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Section)
                .WithMany()
                .HasForeignKey(e => e.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- DropCourseRequest relationship ----------

            modelBuilder.Entity<DropCourseRequest>()
                .HasOne(d => d.Enrollment)
                .WithMany()
                .HasForeignKey(d => d.EnrollmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Timetable relationships ----------

            modelBuilder.Entity<Timetable>()
                .HasOne(t => t.Section)
                .WithMany()
                .HasForeignKey(t => t.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Timetable>()
                .HasOne(t => t.Teacher)
                .WithMany()
                .HasForeignKey(t => t.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Attendance unique index & relationships ----------

            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.SectionId, a.StudentId, a.Date, a.TimetableId })
                .IsUnique();

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Section)
                .WithMany()
                .HasForeignKey(a => a.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Timetable)
                .WithMany()
                .HasForeignKey(a => a.TimetableId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
