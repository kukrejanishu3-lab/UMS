using System.Text.Json.Serialization;

namespace UMS.Application.DTOs.DropCourseRequest
{
    public class DropCourseRequestDto
    {
        [JsonPropertyName("requestId")]
        public Guid Id { get; set; }

        public Guid EnrollmentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public DateOnly RequestDate { get; set; }

        public string? AdminRemarks { get; set; }
    }
}
