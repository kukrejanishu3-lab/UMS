using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Course;
using UMS.Application.Features.Course.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Course.Handlers
{
    // Handles retrieving a course by its ID with detailed active enrolled students list.
    public class GetCourseByIdQueryHandler(IAppDbContext dbContext)
        : IRequestHandler<GetCourseByIdQuery, CourseDto?>
    {
        public async Task<CourseDto?> Handle(
            GetCourseByIdQuery request,
            CancellationToken cancellationToken)
        {
            var course = await dbContext.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

            if (course is null)
            {
                return null;
            }

            var enrolledStudents = await dbContext.Enrollments
                .AsNoTracking()
                .Where(e => e.Section.CourseId == request.CourseId && e.Status == "Enrolled")
                .Include(e => e.Student)
                .Include(e => e.Section)
                .Select(e => new CourseEnrolledStudentDto
                {
                    StudentId = e.StudentId,
                    StudentName = e.Student != null ? $"{e.Student.FirstName} {e.Student.LastName}".Trim() : string.Empty,
                    StudentNumber = e.Student != null ? e.Student.StudentNumber : string.Empty,
                    SectionName = e.Section != null ? e.Section.Name : string.Empty
                })
                .ToListAsync(cancellationToken);

            return new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Code = course.Code,
                CreditHours = course.CreditHours,
                DepartmentId = course.DepartmentId,
                EnrolledStudentsCount = enrolledStudents.Count,
                EnrolledStudents = enrolledStudents
            };
        }
    }
}
