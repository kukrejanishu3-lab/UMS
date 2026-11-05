
namespace UMS.Application.DTOs.DropCourseRequest
{
    public class CreateDropCourseRequestDto
    {
        public Guid EnrollmentId { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
