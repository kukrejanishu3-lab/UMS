
namespace UMS.Application.DTOs.DropCourseRequest
{
    public class UpdateDropCourseRequestDto
    {
        public string Status { get; set; } = "Pending";

        public string? AdminRemarks { get; set; }
    }
}
