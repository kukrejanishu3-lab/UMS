using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.SectionTeacher.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.SectionTeacher.Handlers
{
    // Handles the query to retrieve all teacher-section assignments.
    public class GetAllSectionTeachersQueryHandler
        : IRequestHandler<GetAllSectionTeachersQuery, List<SectionTeacherDto>>
    {
        private readonly IAppDbContext _context;

        public GetAllSectionTeachersQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SectionTeacherDto>> Handle(
            GetAllSectionTeachersQuery request,
            CancellationToken cancellationToken)
        {
            var assignments = await _context.SectionTeachers
                .Include(st => st.Section)
                .Include(st => st.Teacher)
                .ToListAsync(cancellationToken);

            return assignments.Select(st => new SectionTeacherDto
            {
                SectionId = st.SectionId,
                SectionName = st.Section.Name,
                TeacherId = st.TeacherId,
                TeacherName = $"{st.Teacher.FirstName} {st.Teacher.LastName}"
            }).ToList();
        }
    }
}
