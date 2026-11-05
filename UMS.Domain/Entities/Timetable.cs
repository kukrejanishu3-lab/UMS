namespace UMS.Domain.Entities
{
    public class Timetable
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; }
        public Guid TeacherId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string RoomNumber { get; set; } = string.Empty;

        public Section Section { get; set; } = null!;
        public Teacher Teacher { get; set; } = null!;
    }
}