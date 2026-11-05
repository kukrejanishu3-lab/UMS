using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Section;
using UMS.Application.Features.Section.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Section.Handlers
{
    // Handles the command to create multiple sections, skipping duplicates per course.
    public class CreateMultipleSectionsCommandHandler
        : IRequestHandler<CreateMultipleSectionsCommand, CreateMultipleSectionsResultDto>
    {
        private readonly IAppDbContext _context;

        public CreateMultipleSectionsCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<CreateMultipleSectionsResultDto> Handle(
            CreateMultipleSectionsCommand request,
            CancellationToken cancellationToken)
        {
            var addedSections = new List<SectionDto>();
            var rejectedSections = new List<object>();

            foreach (var dto in request.Sections)
            {
                // Check if a section with the same name already exists for this course.
                var sectionExists = await _context.Sections
                    .AnyAsync(
                        s => s.Name.ToLower() == dto.Name.Trim().ToLower()
                          && s.CourseId == dto.CourseId,
                        cancellationToken);

                if (sectionExists)
                {
                    rejectedSections.Add(new
                    {
                        name = dto.Name,
                        courseId = dto.CourseId,
                        message = "This section already exists for this course."
                    });
                    continue;
                }

                var section = new UMS.Domain.Entities.Section
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name.Trim(),
                    Capacity = dto.Capacity,
                    CourseId = dto.CourseId,
                    SemesterId = dto.SemesterId
                };

                _context.Sections.Add(section);
                await _context.SaveChangesAsync(cancellationToken);

                addedSections.Add(new SectionDto
                {
                    Id = section.Id,
                    Name = section.Name,
                    Capacity = section.Capacity,
                    CourseId = section.CourseId,
                    SemesterId = section.SemesterId
                });
            }

            return new CreateMultipleSectionsResultDto
            {
                Message = "Multiple section registration completed.",
                AddedCount = addedSections.Count,
                RejectedCount = rejectedSections.Count,
                AddedSections = addedSections,
                RejectedSections = rejectedSections
            };
        }
    }
}
