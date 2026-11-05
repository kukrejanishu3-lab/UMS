using MediatR;
using UMS.Application.DTOs.Semester;

namespace UMS.Application.Features.Semester.Queries
{
    // Request to retrieve all semesters.
    public class GetSemestersQuery : IRequest<IEnumerable<SemesterDto>>
    {
    }
}
