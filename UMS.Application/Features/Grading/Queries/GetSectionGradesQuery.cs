using MediatR;
using UMS.Application.DTOs.Grading;

namespace UMS.Application.Features.Grading.Queries
{
    // Query to retrieve the awards list / grade sheet of a section
    public record GetSectionGradesQuery(
        Guid SectionId,
        string UserEmail,
        Guid? UserId) : IRequest<SectionGradeSheetDto>;
}
