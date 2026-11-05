using MediatR;
using UMS.Application.DTOs.Teacher;

namespace UMS.Application.Features.Teacher.Queries
{
    // Request to retrieve all teachers.
    public class GetTeachersQuery : IRequest<IEnumerable<TeacherDto>>
    {
    }
}
