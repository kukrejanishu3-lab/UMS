using MediatR;
using UMS.Application.DTOs.DropCourseRequest;

namespace UMS.Application.Features.DropCourseRequest.Queries
{
    // Query to get a single drop-course request by its ID.
    public class GetDropCourseRequestByIdQuery : IRequest<DropCourseRequestDto?>
    {
        public Guid RequestId { get; set; }
    }
}
