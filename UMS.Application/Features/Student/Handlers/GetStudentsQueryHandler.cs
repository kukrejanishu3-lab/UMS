using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Student;
using UMS.Application.Features.Student.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Student.Handlers
{
    // Handles the request to retrieve all active students with full academic details.
    public class GetStudentsQueryHandler(IAppDbContext dbContext)
        : IRequestHandler<GetStudentsQuery, IEnumerable<StudentDto>>
    {
        public async Task<IEnumerable<StudentDto>> Handle(
            GetStudentsQuery request,
            CancellationToken cancellationToken)
        {
            var students = await dbContext.Students
                .AsNoTracking()
                .Include(s => s.Department)
                .Where(s => s.IsActive)
                .ToListAsync(cancellationToken);

            var studentIds = students.Select(s => s.Id).ToList();

            var enrollments = await dbContext.Enrollments
                .AsNoTracking()
                .Where(e => studentIds.Contains(e.StudentId))
                .Include(e => e.Section)
                    .ThenInclude(sec => sec.Course)
                .Include(e => e.Section)
                    .ThenInclude(sec => sec.Semester)
                .ToListAsync(cancellationToken);

            return students.Select(s =>
            {
                var studentEnrollments = enrollments.Where(e => e.StudentId == s.Id).ToList();
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
                    TotalCoursesEnrolled = studentEnrollments.Count(e => e.Status == "Enrolled"),
                    Courses = studentEnrollments.Select(e => new StudentEnrolledCourseDto
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
            }).ToList();
        }
    }
}

