using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Section;
using UMS.Application.Features.Section.Commands;
using UMS.Application.Interfaces;
using UMS.Domain.Entities;

namespace UMS.Application.Features.Section.Handlers
{
    // Handles the command to create a single new section.
    public class CreateSectionCommandHandler : IRequestHandler<CreateSectionCommand, SectionDto>
    {
        private readonly IAppDbContext _context;

        public CreateSectionCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<SectionDto> Handle(
            CreateSectionCommand request,
            CancellationToken cancellationToken)
        {
            var section = new UMS.Domain.Entities.Section
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Capacity = request.Capacity,
                CourseId = request.CourseId,
                SemesterId = request.SemesterId
            };

            _context.Sections.Add(section);
            await _context.SaveChangesAsync(cancellationToken);

            // Reload with related data.
            var created = await _context.Sections
                .Include(s => s.Course)
                .Include(s => s.Semester)
                .FirstOrDefaultAsync(s => s.Id == section.Id, cancellationToken);

            return new SectionDto
            {
                Id = created!.Id,
                Name = created.Name,
                Capacity = created.Capacity,
                CourseId = created.CourseId,
                CourseName = created.Course.Name,
                SemesterId = created.SemesterId,
                SemesterName = created.Semester.Name
            };
        }
    }
}
