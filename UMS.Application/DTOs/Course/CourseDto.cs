using System.Text.Json.Serialization;

namespace UMS.Application.DTOs.Course
{
    public class CourseDto
    {
        [JsonPropertyName("courseId")]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public int CreditHours { get; set; }

        public Guid DepartmentId { get; set; }

        public int EnrolledStudentsCount { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CourseEnrolledStudentDto>? EnrolledStudents { get; set; }
    }

    public class CourseEnrolledStudentDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
    }
}
