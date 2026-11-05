using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Section;
using UMS.Application.Features.Section.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Section.Handlers
{
    // Handles the command to update an existing section.
    public class UpdateSectionCommandHandler : IRequestHandler<UpdateSectionCommand, SectionDto?>
    {
        private readonly IAppDbContext _context;

        public UpdateSectionCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<SectionDto?> Handle(
            UpdateSectionCommand request,
            CancellationToken cancellationToken)
        {
            var section = await _context.Sections
                .FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken);

            if (section is null)
                return null;

            section.Name = request.Name;
            section.Capacity = request.Capacity;
            section.CourseId = request.CourseId;
            section.SemesterId = request.SemesterId;

            await _context.SaveChangesAsync(cancellationToken);

            // Reload with related data.
            var updated = await _context.Sections
                .Include(s => s.Course)
                .Include(s => s.Semester)
                .FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken);

            return new SectionDto
            {
                Id = updated!.Id,
                Name = updated.Name,
                Capacity = updated.Capacity,
                CourseId = updated.CourseId,
                CourseName = updated.Course.Name,
                SemesterId = updated.SemesterId,
                SemesterName = updated.Semester.Name
            };
        }
    }
}
