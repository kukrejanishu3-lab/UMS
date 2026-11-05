using MediatR;
using UMS.Application.DTOs.Semester;

namespace UMS.Application.Features.Semester.Commands
{
    // Request to update an existing semester.
    public class UpdateSemesterCommand(Guid semesterId, UpdateSemesterDto semesterDto) : IRequest<SemesterDto?>
    {
        public Guid SemesterId { get; } = semesterId;
        public UpdateSemesterDto SemesterDto { get; } = semesterDto;
    }
}
