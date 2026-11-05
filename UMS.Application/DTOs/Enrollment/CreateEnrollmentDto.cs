
namespace UMS.Application.DTOs.Enrollment
{
    public class CreateEnrollmentDto
    {
        public Guid StudentId { get; set; }

        public Guid SectionId { get; set; }

        public DateOnly? EnrollmentDate { get; set; }
    }
}
