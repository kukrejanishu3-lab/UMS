
namespace UMS.Application.DTOs.Course
{
    public class UpdateCourseDto
    {
        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public int CreditHours { get; set; }

        public Guid DepartmentId { get; set; }
    }
}
