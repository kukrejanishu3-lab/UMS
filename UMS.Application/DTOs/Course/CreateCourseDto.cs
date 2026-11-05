
namespace UMS.Application.DTOs.Course
{
    public class CreateCourseDto
    {
        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public int CreditHours { get; set; }

        public Guid DepartmentId { get; set; }
    }
}
