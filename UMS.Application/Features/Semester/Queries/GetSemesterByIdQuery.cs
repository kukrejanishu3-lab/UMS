using MediatR;
using UMS.Application.DTOs.Semester;

namespace UMS.Application.Features.Semester.Queries
{
    // Request to retrieve a semester by ID.
    public class GetSemesterByIdQuery(Guid semesterId) : IRequest<SemesterDto?>
    {
        public Guid SemesterId { get; } = semesterId;
    }
}
