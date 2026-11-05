using MediatR;
using UMS.Application.DTOs.Course;

namespace UMS.Application.Features.Course.Commands
{
    // Request to create a new course.
    public class CreateCourseCommand(CreateCourseDto courseDto) : IRequest<CourseDto>
    {
        public CreateCourseDto CourseDto { get; } = courseDto;
    }
}
