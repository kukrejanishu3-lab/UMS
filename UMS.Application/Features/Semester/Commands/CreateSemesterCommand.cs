using MediatR;
using UMS.Application.DTOs.Semester;

namespace UMS.Application.Features.Semester.Commands
{
    // Request to create a new semester.
    public class CreateSemesterCommand(CreateSemesterDto semesterDto) : IRequest<SemesterDto>
    {
        public CreateSemesterDto SemesterDto { get; } = semesterDto;
    }
}
