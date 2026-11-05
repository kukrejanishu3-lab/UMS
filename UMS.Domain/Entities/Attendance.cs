namespace UMS.Domain.Entities
{
    public class Attendance
    {
        public Guid Id { get; set; }

        public Guid SectionId { get; set; }
        public Guid StudentId { get; set; }

        public DateOnly Date { get; set; }

        // Optional link to specific timetable slot (e.g. 09:00 - 10:00)
        public Guid? TimetableId { get; set; }

        // Status: Present, Absent, Late, Excused
        public string Status { get; set; } = "Present";

        public string? Remarks { get; set; }

        // Navigation Properties
        public Section Section { get; set; } = null!;
        public Student Student { get; set; } = null!;
        public Timetable? Timetable { get; set; }
    }
}
