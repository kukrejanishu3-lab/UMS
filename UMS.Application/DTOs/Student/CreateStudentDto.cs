
namespace UMS.Application.DTOs.Student
{
    public class CreateStudentDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string StudentNumber { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public Guid DepartmentId { get; set; }
    }
}
