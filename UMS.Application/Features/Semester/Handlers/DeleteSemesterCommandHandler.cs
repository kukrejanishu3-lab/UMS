using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.Semester.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Semester.Handlers
{
    // Handles deleting a semester by ID.
    public class DeleteSemesterCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<DeleteSemesterCommand, bool>
    {
        public async Task<bool> Handle(
            DeleteSemesterCommand request,
            CancellationToken cancellationToken)
        {
            var semester = await dbContext.Semesters
                .FirstOrDefaultAsync(s => s.Id == request.SemesterId, cancellationToken);

            if (semester is null)
            {
                return false;
            }

            dbContext.Semesters.Remove(semester);
            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
