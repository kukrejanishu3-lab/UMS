using MediatR;
using UMS.Application.DTOs.Course;

namespace UMS.Application.Features.Course.Queries
{
    // Request to retrieve a course by ID.
    public class GetCourseByIdQuery(Guid courseId) : IRequest<CourseDto?>
    {
        public Guid CourseId { get; } = courseId;
    }
}
