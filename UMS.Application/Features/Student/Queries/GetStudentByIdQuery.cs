
using MediatR;
using UMS.Application.DTOs.Student;

namespace UMS.Application.Features.Student.Queries
{
    // Request to retrieve a student by ID.
    public class GetStudentByIdQuery : IRequest<StudentDto?>
    {
        public Guid StudentId { get; set; }

        public GetStudentByIdQuery(Guid studentId)
        {
            StudentId = studentId;
        }
    }
}
