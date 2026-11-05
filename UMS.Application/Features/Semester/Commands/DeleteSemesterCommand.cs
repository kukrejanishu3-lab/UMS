using MediatR;

namespace UMS.Application.Features.Semester.Commands
{
    // Request to delete a semester by ID.
    public class DeleteSemesterCommand(Guid semesterId) : IRequest<bool>
    {
        public Guid SemesterId { get; } = semesterId;
    }
}
