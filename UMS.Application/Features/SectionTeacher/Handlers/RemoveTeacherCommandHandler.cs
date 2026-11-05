using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.SectionTeacher.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.SectionTeacher.Handlers
{
    // Handles the command to remove a teacher from a section.
    public class RemoveTeacherCommandHandler : IRequestHandler<RemoveTeacherCommand, bool>
    {
        private readonly IAppDbContext _context;

        public RemoveTeacherCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(
            RemoveTeacherCommand request,
            CancellationToken cancellationToken)
        {
            var assignment = await _context.SectionTeachers
                .FirstOrDefaultAsync(
                    st => st.SectionId == request.SectionId
                       && st.TeacherId == request.TeacherId,
                    cancellationToken);

            if (assignment is null)
                return false;

            _context.SectionTeachers.Remove(assignment);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
