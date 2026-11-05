using MediatR;

namespace UMS.Application.Features.Teacher.Commands
{
    // Request to delete a teacher by ID.
    public class DeleteTeacherCommand(Guid teacherId) : IRequest<bool>
    {
        public Guid TeacherId { get; } = teacherId;
    }
}
