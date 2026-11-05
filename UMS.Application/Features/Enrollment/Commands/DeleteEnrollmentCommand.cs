using MediatR;

namespace UMS.Application.Features.Enrollment.Commands
{
    // Command to delete an enrollment by its ID.
    public class DeleteEnrollmentCommand : IRequest<bool>
    {
        public Guid EnrollmentId { get; set; }
    }
}
