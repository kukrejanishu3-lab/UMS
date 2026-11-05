
namespace UMS.Application.DTOs.Timetable
{
    public class CreateTimetableDto
    {
        public Guid SectionId { get; set; }

        public Guid TeacherId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string RoomNumber { get; set; } = string.Empty;
    }
}
