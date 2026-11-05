using MediatR;
using UMS.Application.DTOs.Enrollment;

namespace UMS.Application.Features.Enrollment.Queries
{
    // Query to get a single enrollment by its ID.
    public class GetEnrollmentByIdQuery : IRequest<EnrollmentDto?>
    {
        public Guid EnrollmentId { get; set; }
    }
}
