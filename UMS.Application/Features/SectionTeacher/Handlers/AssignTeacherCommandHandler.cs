using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.SectionTeacher.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.SectionTeacher.Handlers
{
    // Handles the command to assign a teacher to a section.
    public class AssignTeacherCommandHandler : IRequestHandler<AssignTeacherCommand, string>
    {
        private readonly IAppDbContext _context;

        public AssignTeacherCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(
            AssignTeacherCommand request,
            CancellationToken cancellationToken)
        {
            var sectionExists = await _context.Sections
                .AnyAsync(s => s.Id == request.SectionId, cancellationToken);

            if (!sectionExists)
                return "SectionNotFound";

            var teacherExists = await _context.Teachers
                .AnyAsync(t => t.Id == request.TeacherId, cancellationToken);

            if (!teacherExists)
                return "TeacherNotFound";

            var alreadyAssigned = await _context.SectionTeachers
                .AnyAsync(
                    st => st.SectionId == request.SectionId
                       && st.TeacherId == request.TeacherId,
                    cancellationToken);

            if (alreadyAssigned)
                return "AlreadyAssigned";

            var assignment = new UMS.Domain.Entities.SectionTeacher
            {
                SectionId = request.SectionId,
                TeacherId = request.TeacherId
            };

            _context.SectionTeachers.Add(assignment);
            await _context.SaveChangesAsync(cancellationToken);

            return "Success";
        }
    }
}
