using MediatR;
using UMS.Application.DTOs.DropCourseRequest;

namespace UMS.Application.Features.DropCourseRequest.Commands
{
    // Command to approve or reject a pending drop-course request.
    public class UpdateDropCourseRequestStatusCommand : IRequest<DropCourseRequestDto?>
    {
        public Guid RequestId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? AdminRemarks { get; set; }
    }
}
