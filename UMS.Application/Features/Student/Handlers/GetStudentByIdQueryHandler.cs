using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Student;
using UMS.Application.Features.Student.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Student.Handlers
{
    // Handles the request to retrieve an active student by ID with full academic details.
    public class GetStudentByIdQueryHandler(IAppDbContext dbContext)
        : IRequestHandler<GetStudentByIdQuery, StudentDto?>
    {
        public async Task<StudentDto?> Handle(
            GetStudentByIdQuery request,
            CancellationToken cancellationToken)
        {
            var s = await dbContext.Students
                .AsNoTracking()
                .Include(st => st.Department)
                .FirstOrDefaultAsync(x => x.Id == request.StudentId && x.IsActive, cancellationToken);

            if (s is null) return null;

            var enrollments = await dbContext.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == s.Id)
                .Include(e => e.Section)
                    .ThenInclude(sec => sec.Course)
                .Include(e => e.Section)
                    .ThenInclude(sec => sec.Semester)
                .ToListAsync(cancellationToken);

            return new StudentDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                StudentNumber = s.StudentNumber,
                DateOfBirth = s.DateOfBirth,
                PhoneNumber = s.PhoneNumber,
                DepartmentId = s.DepartmentId,
                DepartmentName = s.Department != null ? s.Department.Name : string.Empty,
                IsActive = s.IsActive,
                TotalCoursesEnrolled = enrollments.Count(e => e.Status == "Enrolled"),
                Courses = enrollments.Select(e => new StudentEnrolledCourseDto
                {
                    CourseId = e.Section != null ? e.Section.CourseId : Guid.Empty,
                    CourseName = e.Section?.Course?.Name ?? string.Empty,
                    CourseCode = e.Section?.Course?.Code ?? string.Empty,
                    CreditHours = e.Section?.Course?.CreditHours ?? 0,
                    SectionId = e.SectionId,
                    SectionName = e.Section?.Name ?? string.Empty,
                    SemesterName = e.Section?.Semester?.Name ?? string.Empty,
                    EnrollmentDate = e.EnrollmentDate,
                    EnrollmentStatus = e.Status,
                    Grade = e.Grade,
                    GradePoints = e.GradePoints
                }).ToList()
            };
        }
    }
}

