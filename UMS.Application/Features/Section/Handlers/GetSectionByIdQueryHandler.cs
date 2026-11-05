using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Section;
using UMS.Application.Features.Section.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Section.Handlers
{
    // Handles the query to retrieve a single section by its ID with enrolled students list.
    public class GetSectionByIdQueryHandler : IRequestHandler<GetSectionByIdQuery, SectionDto?>
    {
        private readonly IAppDbContext _context;

        public GetSectionByIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<SectionDto?> Handle(
            GetSectionByIdQuery request,
            CancellationToken cancellationToken)
        {
            var section = await _context.Sections
                .AsNoTracking()
                .Include(s => s.Course)
                .Include(s => s.Semester)
                .FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken);

            if (section is null)
                return null;

            var enrolledStudents = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.SectionId == request.SectionId && e.Status == "Enrolled")
                .Include(e => e.Student)
                .Select(e => new SectionEnrolledStudentDto
                {
                    StudentId = e.StudentId,
                    StudentName = e.Student != null ? $"{e.Student.FirstName} {e.Student.LastName}".Trim() : string.Empty,
                    StudentNumber = e.Student != null ? e.Student.StudentNumber : string.Empty
                })
                .ToListAsync(cancellationToken);

            return new SectionDto
            {
                Id = section.Id,
                Name = section.Name,
                Capacity = section.Capacity,
                EnrolledStudentsCount = enrolledStudents.Count,
                CourseId = section.CourseId,
                CourseName = section.Course.Name,
                SemesterId = section.SemesterId,
                SemesterName = section.Semester.Name,
                EnrolledStudents = enrolledStudents
            };
        }
    }
}

