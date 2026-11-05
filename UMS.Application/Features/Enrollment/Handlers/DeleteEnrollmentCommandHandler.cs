using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.Enrollment.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Enrollment.Handlers
{
    // Handles the command to delete an enrollment by its ID.
    public class DeleteEnrollmentCommandHandler : IRequestHandler<DeleteEnrollmentCommand, bool>
    {
        private readonly IAppDbContext _context;

        public DeleteEnrollmentCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(
            DeleteEnrollmentCommand request,
            CancellationToken cancellationToken)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

            if (enrollment is null)
                return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
