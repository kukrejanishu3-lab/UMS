using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Course;
using UMS.Application.Features.Course.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Course.Handlers
{
    // Handles updating an existing course.
    public class UpdateCourseCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<UpdateCourseCommand, CourseDto?>
    {
        public async Task<CourseDto?> Handle(
            UpdateCourseCommand request,
            CancellationToken cancellationToken)
        {
            var existingCourse = await dbContext.Courses
                .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

            if (existingCourse is null)
            {
                return null;
            }

            // Checks whether the updated code is already taken by another course.
            var codeExists = await dbContext.Courses
                .AnyAsync(c => c.Code == request.CourseDto.Code && c.Id != request.CourseId, cancellationToken);

            if (codeExists)
            {
                throw new InvalidOperationException($"Course code {request.CourseDto.Code} already exists.");
            }

            // Checks whether the department exists.
            var departmentExists = await dbContext.Departments
                .AnyAsync(d => d.Id == request.CourseDto.DepartmentId, cancellationToken);

            if (!departmentExists)
            {
                throw new InvalidOperationException($"Department with ID {request.CourseDto.DepartmentId} does not exist.");
            }

            existingCourse.Name = request.CourseDto.Name;
            existingCourse.Code = request.CourseDto.Code;
            existingCourse.CreditHours = request.CourseDto.CreditHours;
            existingCourse.DepartmentId = request.CourseDto.DepartmentId;

            await dbContext.SaveChangesAsync(cancellationToken);

            return new CourseDto
            {
                Id = existingCourse.Id,
                Name = existingCourse.Name,
                Code = existingCourse.Code,
                CreditHours = existingCourse.CreditHours,
                DepartmentId = existingCourse.DepartmentId
            };
        }
    }
}
