using MediatR;
using UMS.Application.DTOs.Course;

namespace UMS.Application.Features.Course.Commands
{
    // Request to register multiple courses.
    public class CreateMultipleCoursesCommand(List<CreateCourseDto> courses)
        : IRequest<(List<CourseDto> AddedCourses, List<object> RejectedCourses)>
    {
        public List<CreateCourseDto> Courses { get; } = courses;
    }
}
