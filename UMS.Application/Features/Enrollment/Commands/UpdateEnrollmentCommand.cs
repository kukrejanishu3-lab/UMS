using MediatR;
using UMS.Application.DTOs.Enrollment;

namespace UMS.Application.Features.Enrollment.Commands
{
    // Command to update an existing enrollment.
    public class UpdateEnrollmentCommand : IRequest<EnrollmentDto?>
    {
        public Guid EnrollmentId { get; set; }
        public Guid StudentId { get; set; }
        public Guid SectionId { get; set; }
        public DateOnly EnrollmentDate { get; set; }
        public string Status { get; set; } = "Enrolled";
    }
}
