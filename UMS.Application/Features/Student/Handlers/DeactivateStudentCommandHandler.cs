using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.Student.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Student.Handlers
{
    // Handles the request to deactivate a student.
    public class DeactivateStudentCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<DeactivateStudentCommand, bool>
    {
        public async Task<bool> Handle(
            DeactivateStudentCommand request,
            CancellationToken cancellationToken)
        {
            var student = await dbContext.Students
                .FirstOrDefaultAsync(x => x.Id == request.StudentId, cancellationToken);

            if (student is null)
            {
                return false;
            }

            // Marks the student as inactive instead of permanently deleting them.
            student.IsActive = false;

            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}