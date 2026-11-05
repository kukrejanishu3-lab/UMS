using System.Text.Json.Serialization;

namespace UMS.Application.DTOs.Timetable
{
    public class TimetableDto
    {
        [JsonPropertyName("timetableId")]
        public Guid Id { get; set; }

        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;

        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;

        public string Day { get; set; } = string.Empty;
        public DayOfWeek DayOfWeek { get; set; }

        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;

        public string Duration { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
    }
}
