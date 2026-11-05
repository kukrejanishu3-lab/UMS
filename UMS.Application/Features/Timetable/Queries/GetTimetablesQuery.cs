using MediatR;
using UMS.Application.DTOs.Timetable;

namespace UMS.Application.Features.Timetable.Queries
{
    // Query to retrieve all timetable entries.
    public class GetTimetablesQuery : IRequest<IEnumerable<TimetableDto>>
    {
    }
}
