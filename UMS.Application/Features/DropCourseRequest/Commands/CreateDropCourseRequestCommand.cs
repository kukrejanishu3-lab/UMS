using MediatR;
using UMS.Application.DTOs.DropCourseRequest;

namespace UMS.Application.Features.DropCourseRequest.Commands
{
    // Command to submit a new drop-course request.
    public class CreateDropCourseRequestCommand : IRequest<DropCourseRequestDto>
    {
        public Guid EnrollmentId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
