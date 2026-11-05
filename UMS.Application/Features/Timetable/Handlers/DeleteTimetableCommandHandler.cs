using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.Timetable.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Timetable.Handlers
{
    // Handles deleting a timetable entry by ID.
    public class DeleteTimetableCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<DeleteTimetableCommand, bool>
    {
        public async Task<bool> Handle(
            DeleteTimetableCommand request,
            CancellationToken cancellationToken)
        {
            var existing = await dbContext.Timetables
                .FirstOrDefaultAsync(t => t.Id == request.TimetableId, cancellationToken);

            if (existing is null)
            {
                return false;
            }

            dbContext.Timetables.Remove(existing);
            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
