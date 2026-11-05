using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.SectionTeacher.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.SectionTeacher.Handlers
{
    // Handles the query to retrieve all teachers assigned to a specific section.
    public class GetTeachersBySectionIdQueryHandler
        : IRequestHandler<GetTeachersBySectionIdQuery, List<SectionTeacherBySectionDto>?>
    {
        private readonly IAppDbContext _context;

        public GetTeachersBySectionIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SectionTeacherBySectionDto>?> Handle(
            GetTeachersBySectionIdQuery request,
            CancellationToken cancellationToken)
        {
            // First verify the section exists.
            var sectionExists = await _context.Sections
                .AnyAsync(s => s.Id == request.SectionId, cancellationToken);

            if (!sectionExists)
                return null;

            var assignments = await _context.SectionTeachers
                .Include(st => st.Teacher)
                .Where(st => st.SectionId == request.SectionId)
                .ToListAsync(cancellationToken);

            return assignments.Select(st => new SectionTeacherBySectionDto
            {
                SectionId = st.SectionId,
                TeacherId = st.TeacherId,
                TeacherName = $"{st.Teacher.FirstName} {st.Teacher.LastName}"
            }).ToList();
        }
    }
}
