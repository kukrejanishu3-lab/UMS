using MediatR;
using UMS.Application.DTOs.DropCourseRequest;

namespace UMS.Application.Features.DropCourseRequest.Queries
{
    // Query to get all drop-course requests.
    public class GetDropCourseRequestsQuery : IRequest<List<DropCourseRequestDto>>
    {
    }
}
