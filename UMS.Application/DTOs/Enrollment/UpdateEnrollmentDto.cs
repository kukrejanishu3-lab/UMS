
namespace UMS.Application.DTOs.Enrollment
{
    public class UpdateEnrollmentDto
    {
        public Guid StudentId { get; set; }

        public Guid SectionId { get; set; }

        public DateOnly EnrollmentDate { get; set; }

        public string Status { get; set; } = "Enrolled";
    }
}
