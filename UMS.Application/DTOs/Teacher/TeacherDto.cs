using System.Text.Json.Serialization;

namespace UMS.Application.DTOs.Teacher
{
    public class TeacherDto
    {
        [JsonPropertyName("teacherId")]
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string EmployeeNumber { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public Guid DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public int TotalAssignedSections { get; set; }

        public List<TeacherAssignedSectionDto> AssignedSections { get; set; } = new();

        public List<TeacherScheduleSlotDto> TeachingSchedule { get; set; } = new();
    }

    public class TeacherAssignedSectionDto
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
    }

    public class TeacherScheduleSlotDto
    {
        public Guid TimetableId { get; set; }
        public string Day { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
    }
}

