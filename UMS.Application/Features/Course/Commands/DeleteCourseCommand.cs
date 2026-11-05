using MediatR;

namespace UMS.Application.Features.Course.Commands
{
    // Request to delete a course by ID.
    public class DeleteCourseCommand(Guid courseId) : IRequest<bool>
    {
        public Guid CourseId { get; } = courseId;
    }
}
