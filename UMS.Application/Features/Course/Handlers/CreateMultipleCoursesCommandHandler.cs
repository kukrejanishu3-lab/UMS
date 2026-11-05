using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Course;
using UMS.Application.Features.Course.Commands;
using UMS.Application.Interfaces;
using CourseEntity = UMS.Domain.Entities.Course;

namespace UMS.Application.Features.Course.Handlers
{
    // Handles bulk registration of courses.
    public class CreateMultipleCoursesCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<
            CreateMultipleCoursesCommand,
            (List<CourseDto> AddedCourses, List<object> RejectedCourses)>
    {
        public async Task<(
            List<CourseDto> AddedCourses,
            List<object> RejectedCourses)> Handle(
                CreateMultipleCoursesCommand request,
                CancellationToken cancellationToken)
        {
            var addedCourses = new List<CourseDto>();
            var rejectedCourses = new List<object>();

            var courseCodesInRequest = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var requestCodes = request.Courses.Select(x => x.Code).Distinct().ToList();
            var existingCodes = (await dbContext.Courses
                .Where(c => requestCodes.Contains(c.Code))
                .Select(c => c.Code)
                .ToListAsync(cancellationToken))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var validDepartmentIds = (await dbContext.Departments
                .Select(d => d.Id)
                .ToListAsync(cancellationToken))
                .ToHashSet();

            foreach (var dto in request.Courses)
            {
                // Checks for duplicate course codes within the request payload.
                if (!courseCodesInRequest.Add(dto.Code))
                {
                    rejectedCourses.Add(new
                    {
                        code = dto.Code,
                        message = $"Course code {dto.Code} is duplicated in this request."
                    });
                    continue;
                }

                // Checks whether the course code already exists in the database.
                if (existingCodes.Contains(dto.Code))
                {
                    rejectedCourses.Add(new
                    {
                        code = dto.Code,
                        message = $"Course code {dto.Code} already exists."
                    });
                    continue;
                }

                // Checks whether the department exists.
                if (!validDepartmentIds.Contains(dto.DepartmentId))
                {
                    rejectedCourses.Add(new
                    {
                        code = dto.Code,
                        message = $"Department with ID {dto.DepartmentId} does not exist."
                    });
                    continue;
                }

                var course = new CourseEntity
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Code = dto.Code,
                    CreditHours = dto.CreditHours,
                    DepartmentId = dto.DepartmentId
                };

                await dbContext.Courses.AddAsync(course, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                // Add to existingCodes set so subsequent items in the batch also respect it
                existingCodes.Add(dto.Code);

                addedCourses.Add(new CourseDto
                {
                    Id = course.Id,
                    Name = course.Name,
                    Code = course.Code,
                    CreditHours = course.CreditHours,
                    DepartmentId = course.DepartmentId
                });
            }

            return (addedCourses, rejectedCourses);
        }
    }
}
