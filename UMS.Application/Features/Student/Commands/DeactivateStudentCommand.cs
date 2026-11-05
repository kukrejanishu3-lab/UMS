using MediatR;

namespace UMS.Application.Features.Student.Commands
{
    // Request to deactivate a student by ID.
    public class DeactivateStudentCommand : IRequest<bool>
    {
        public Guid StudentId { get; set; }

        public DeactivateStudentCommand(Guid studentId)
        {
            StudentId = studentId;
        }
    }
}