using MediatR;
using UMS.Application.DTOs.Enrollment;

namespace UMS.Application.Features.Enrollment.Commands
{
    // Command to create a new enrollment.
    public class CreateEnrollmentCommand : IRequest<EnrollmentDto>
    {
        public Guid StudentId { get; set; }
        public Guid SectionId { get; set; }
        public DateOnly EnrollmentDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public string Status { get; set; } = "Enrolled";
    }
}
