using Microsoft.EntityFrameworkCore;
using UMS.Domain.Entities;

namespace UMS.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Department> Departments { get; }
        DbSet<Student> Students { get; }
        DbSet<Teacher> Teachers { get; }
        DbSet<Course> Courses { get; }
        DbSet<Section> Sections { get; }
        DbSet<Semester> Semesters { get; }
        DbSet<Enrollment> Enrollments { get; }
        DbSet<DropCourseRequest> DropCourseRequests { get; }
        DbSet<Timetable> Timetables { get; }
        DbSet<User> Users { get; }
        DbSet<SectionTeacher> SectionTeachers { get; }
        DbSet<Attendance> Attendances { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
