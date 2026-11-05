using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Course;
using UMS.Application.Features.Course.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Course.Handlers
{
    // Handles retrieving all courses with active enrolled student counts.
    public class GetCoursesQueryHandler(IAppDbContext dbContext)
        : IRequestHandler<GetCoursesQuery, IEnumerable<CourseDto>>
    {
        public async Task<IEnumerable<CourseDto>> Handle(
            GetCoursesQuery request,
            CancellationToken cancellationToken)
        {
            var courses = await dbContext.Courses
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var activeEnrollments = await dbContext.Enrollments
                .AsNoTracking()
                .Where(e => e.Status == "Enrolled")
                .Select(e => new { e.Section.CourseId, e.StudentId })
                .ToListAsync(cancellationToken);

            var countsByCourse = activeEnrollments
                .GroupBy(e => e.CourseId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.StudentId).Distinct().Count());

            return courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                CreditHours = c.CreditHours,
                DepartmentId = c.DepartmentId,
                EnrolledStudentsCount = countsByCourse.TryGetValue(c.Id, out var count) ? count : 0
            }).ToList();
        }
    }
}
