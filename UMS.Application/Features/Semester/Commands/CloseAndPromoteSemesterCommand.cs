using MediatR;
using UMS.Application.DTOs.Semester;

namespace UMS.Application.Features.Semester.Commands
{
    // Command for Admin to formally conclude a semester, freeze results, and promote students
    public record CloseAndPromoteSemesterCommand(
        Guid SemesterId,
        string AdminEmail,
        Guid? AdminUserId) : IRequest<CloseSemesterResultDto>;
}

