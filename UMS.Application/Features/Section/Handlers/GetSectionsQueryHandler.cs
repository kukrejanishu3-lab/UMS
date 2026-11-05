using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Section;
using UMS.Application.Features.Section.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Section.Handlers
{
    // Handles the query to retrieve all sections with active enrolled student counts.
    public class GetSectionsQueryHandler : IRequestHandler<GetSectionsQuery, List<SectionDto>>
    {
        private readonly IAppDbContext _context;

        public GetSectionsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SectionDto>> Handle(
            GetSectionsQuery request,
            CancellationToken cancellationToken)
        {
            var sections = await _context.Sections
                .AsNoTracking()
                .Include(s => s.Course)
                .Include(s => s.Semester)
                .ToListAsync(cancellationToken);

            var activeEnrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.Status == "Enrolled")
                .Select(e => new { e.SectionId, e.StudentId })
                .ToListAsync(cancellationToken);

            var countsBySection = activeEnrollments
                .GroupBy(e => e.SectionId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.StudentId).Distinct().Count());

            return sections.Select(s => new SectionDto
            {
                Id = s.Id,
                Name = s.Name,
                Capacity = s.Capacity,
                EnrolledStudentsCount = countsBySection.TryGetValue(s.Id, out var count) ? count : 0,
                CourseId = s.CourseId,
                CourseName = s.Course.Name,
                SemesterId = s.SemesterId,
                SemesterName = s.Semester.Name
            }).ToList();
        }
    }
}

