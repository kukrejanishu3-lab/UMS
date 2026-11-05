using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Course;
using UMS.Application.Features.Course.Commands;
using UMS.Application.Interfaces;
using CourseEntity = UMS.Domain.Entities.Course;

namespace UMS.Application.Features.Course.Handlers
{
    // Handles creating a new course.
    public class CreateCourseCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<CreateCourseCommand, CourseDto>
    {
        public async Task<CourseDto> Handle(
            CreateCourseCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.CourseDto;

            var codeExists = await dbContext.Courses
                .AnyAsync(c => c.Code == dto.Code, cancellationToken);

            if (codeExists)
            {
                throw new InvalidOperationException($"Course code {dto.Code} already exists.");
            }

            var departmentExists = await dbContext.Departments
                .AnyAsync(d => d.Id == dto.DepartmentId, cancellationToken);

            if (!departmentExists)
            {
                throw new InvalidOperationException($"Department with ID {dto.DepartmentId} does not exist.");
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

            return new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Code = course.Code,
                CreditHours = course.CreditHours,
                DepartmentId = course.DepartmentId
            };
        }
    }
}
