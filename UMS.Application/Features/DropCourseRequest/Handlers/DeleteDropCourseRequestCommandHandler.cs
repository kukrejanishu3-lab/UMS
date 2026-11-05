using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.DropCourseRequest.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.DropCourseRequest.Handlers
{
    // Handles the command to delete a drop-course request by ID.
    public class DeleteDropCourseRequestCommandHandler
        : IRequestHandler<DeleteDropCourseRequestCommand, bool>
    {
        private readonly IAppDbContext _context;

        public DeleteDropCourseRequestCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(
            DeleteDropCourseRequestCommand request,
            CancellationToken cancellationToken)
        {
            var dropRequest = await _context.DropCourseRequests
                .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

            if (dropRequest is null)
                return false;

            _context.DropCourseRequests.Remove(dropRequest);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
