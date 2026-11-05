using System.Text.Json.Serialization;

namespace UMS.Domain.Entities
{
    public class Student
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

        public Department Department { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        // Semester Progression tracking
        public int CurrentSemester { get; set; } = 1;
        public string AcademicStanding { get; set; } = "Good Standing"; // Good Standing, Academic Probation Warning, Dean's Honor List
        public decimal? CGPA { get; set; }
    }
}
