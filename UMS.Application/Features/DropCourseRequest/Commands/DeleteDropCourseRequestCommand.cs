using MediatR;

namespace UMS.Application.Features.DropCourseRequest.Commands
{
    // Command to delete a drop-course request by its ID.
    public class DeleteDropCourseRequestCommand : IRequest<bool>
    {
        public Guid RequestId { get; set; }
    }
}
