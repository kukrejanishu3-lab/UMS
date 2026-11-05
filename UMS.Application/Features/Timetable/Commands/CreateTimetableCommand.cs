using MediatR;
using UMS.Application.DTOs.Timetable;

namespace UMS.Application.Features.Timetable.Commands
{
    // Command to create a new timetable entry.
    public class CreateTimetableCommand : IRequest<TimetableDto>
    {
        public Guid SectionId { get; set; }
        public Guid TeacherId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
    }
}
