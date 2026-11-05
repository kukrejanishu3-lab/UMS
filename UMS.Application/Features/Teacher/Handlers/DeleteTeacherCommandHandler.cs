using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.Teacher.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Teacher.Handlers
{
    // Handles deleting a teacher by ID.
    public class DeleteTeacherCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<DeleteTeacherCommand, bool>
    {
        public async Task<bool> Handle(
            DeleteTeacherCommand request,
            CancellationToken cancellationToken)
        {
            var teacher = await dbContext.Teachers
                .FirstOrDefaultAsync(t => t.Id == request.TeacherId, cancellationToken);

            if (teacher is null)
            {
                return false;
            }

            dbContext.Teachers.Remove(teacher);
            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
