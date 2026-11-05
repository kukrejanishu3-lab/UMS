using System.Text.Json.Serialization;

namespace UMS.Application.DTOs.Enrollment
{
    public class EnrollmentDto
    {
        [JsonPropertyName("enrollmentId")]
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;

        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;

        public DateOnly EnrollmentDate { get; set; }

        public string Status { get; set; } = "Enrolled";

        public string? Grade { get; set; }
        public decimal? GradePoints { get; set; }
    }
}
