using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.Course.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Course.Handlers
{
    // Handles deleting a course by ID.
    public class DeleteCourseCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<DeleteCourseCommand, bool>
    {
        public async Task<bool> Handle(
            DeleteCourseCommand request,
            CancellationToken cancellationToken)
        {
            var course = await dbContext.Courses
                .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

            if (course is null)
            {
                return false;
            }

            dbContext.Courses.Remove(course);
            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
