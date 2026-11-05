using MediatR;
using UMS.Application.DTOs.Course;

namespace UMS.Application.Features.Course.Commands
{
    // Request to update an existing course.
    public class UpdateCourseCommand(Guid courseId, UpdateCourseDto courseDto) : IRequest<CourseDto?>
    {
        public Guid CourseId { get; } = courseId;
        public UpdateCourseDto CourseDto { get; } = courseDto;
    }
}
