
namespace UMS.Application.DTOs.Teacher
{
    public class CreateTeacherDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string EmployeeNumber { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public Guid DepartmentId { get; set; }
    }
}
