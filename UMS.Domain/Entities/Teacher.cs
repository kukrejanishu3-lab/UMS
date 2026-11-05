using System.Text.Json.Serialization;

namespace UMS.Domain.Entities
{
    public class Teacher
    {
        [JsonPropertyName("teacherId")]
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmployeeNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public Guid DepartmentId { get; set; }

        public Department Department { get; set; } = null!;
    }
}
