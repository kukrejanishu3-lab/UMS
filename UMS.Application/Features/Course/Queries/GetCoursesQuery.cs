using MediatR;
using UMS.Application.DTOs.Course;

namespace UMS.Application.Features.Course.Queries
{
    // Request to retrieve all courses.
    public class GetCoursesQuery : IRequest<IEnumerable<CourseDto>>
    {
    }
}
