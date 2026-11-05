using MediatR;
using UMS.Application.DTOs.Enrollment;

namespace UMS.Application.Features.Enrollment.Queries
{
    // Query to get all enrollments.
    public class GetEnrollmentsQuery : IRequest<List<EnrollmentDto>>
    {
    }
}
