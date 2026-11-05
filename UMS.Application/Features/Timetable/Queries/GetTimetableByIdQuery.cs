using MediatR;
using UMS.Application.DTOs.Timetable;

namespace UMS.Application.Features.Timetable.Queries
{
    // Query to retrieve a timetable entry by its ID.
    public class GetTimetableByIdQuery : IRequest<TimetableDto?>
    {
        public Guid TimetableId { get; set; }

        public GetTimetableByIdQuery()
        {
        }

        public GetTimetableByIdQuery(Guid timetableId)
        {
            TimetableId = timetableId;
        }
    }
}
