using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Enrollment;
using UMS.Application.Features.Enrollment.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Enrollment.Handlers
{
    // Handles the query to retrieve all enrollments with student, section, and course details.
    public class GetEnrollmentsQueryHandler : IRequestHandler<GetEnrollmentsQuery, List<EnrollmentDto>>
    {
        private readonly IAppDbContext _context;

        public GetEnrollmentsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EnrollmentDto>> Handle(
            GetEnrollmentsQuery request,
            CancellationToken cancellationToken)
        {
            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Section)
                    .ThenInclude(s => s.Course)
                .ToListAsync(cancellationToken);

            return enrollments.Select(e => new EnrollmentDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                StudentName = e.Student != null ? $"{e.Student.FirstName} {e.Student.LastName}".Trim() : string.Empty,
                StudentNumber = e.Student?.StudentNumber ?? string.Empty,
                SectionId = e.SectionId,
                SectionName = e.Section?.Name ?? string.Empty,
                CourseName = e.Section?.Course?.Name ?? string.Empty,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                Grade = e.Grade,
                GradePoints = e.GradePoints
            }).ToList();
        }
    }
}
