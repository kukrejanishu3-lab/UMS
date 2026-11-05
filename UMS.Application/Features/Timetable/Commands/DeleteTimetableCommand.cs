using MediatR;

namespace UMS.Application.Features.Timetable.Commands
{
    // Command to delete a timetable entry by its ID.
    public class DeleteTimetableCommand : IRequest<bool>
    {
        public Guid TimetableId { get; set; }
    }
}
