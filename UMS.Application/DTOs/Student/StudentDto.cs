using System.Text.Json.Serialization;

namespace UMS.Application.DTOs.Student
{
    public class StudentDto
    {
        [JsonPropertyName("studentId")]
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string StudentNumber { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public Guid DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public int TotalCoursesEnrolled { get; set; }

        public List<StudentEnrolledCourseDto> Courses { get; set; } = new();
    }

    public class StudentEnrolledCourseDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string SemesterName { get; set; } = string.Empty;
        public DateOnly EnrollmentDate { get; set; }
        public string EnrollmentStatus { get; set; } = string.Empty;
        public string? Grade { get; set; }
        public decimal? GradePoints { get; set; }
    }
}

