using System.Text.Json.Serialization;

namespace UMS.Application.DTOs.Section
{
    public class SectionDto
    {
        [JsonPropertyName("sectionId")]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public int EnrolledStudentsCount { get; set; }

        public int AvailableSeats => Capacity - EnrolledStudentsCount;

        public Guid CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public Guid SemesterId { get; set; }

        public string SemesterName { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<SectionEnrolledStudentDto>? EnrolledStudents { get; set; }
    }

    public class SectionEnrolledStudentDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
    }
}