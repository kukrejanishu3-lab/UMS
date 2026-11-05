
using MediatR;
using UMS.Application.DTOs.Student;

namespace UMS.Application.Features.Student.Queries
{
    // Request to retrieve all students.
    public class GetStudentsQuery : IRequest<IEnumerable<StudentDto>>
    {
    }
}
