using MediatR;
using UMS.Application.DTOs.Teacher;

namespace UMS.Application.Features.Teacher.Queries
{
    // Request to retrieve a teacher by ID.
    public class GetTeacherByIdQuery(Guid teacherId) : IRequest<TeacherDto?>
    {
        public Guid TeacherId { get; } = teacherId;
    }
}
