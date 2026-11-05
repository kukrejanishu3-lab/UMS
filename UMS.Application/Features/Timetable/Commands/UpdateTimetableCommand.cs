using MediatR;
using UMS.Application.DTOs.Timetable;

namespace UMS.Application.Features.Timetable.Commands
{
    // Command to update an existing timetable entry.
    public class UpdateTimetableCommand : IRequest<TimetableDto?>
    {
        public Guid TimetableId { get; set; }
        public Guid SectionId { get; set; }
        public Guid TeacherId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
    }
}
