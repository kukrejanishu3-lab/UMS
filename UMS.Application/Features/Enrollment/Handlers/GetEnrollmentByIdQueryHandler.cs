using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Enrollment;
using UMS.Application.Features.Enrollment.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Enrollment.Handlers
{
    // Handles the query to retrieve a single enrollment by its ID with rich details.
    public class GetEnrollmentByIdQueryHandler : IRequestHandler<GetEnrollmentByIdQuery, EnrollmentDto?>
    {
        private readonly IAppDbContext _context;

        public GetEnrollmentByIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<EnrollmentDto?> Handle(
            GetEnrollmentByIdQuery request,
            CancellationToken cancellationToken)
        {
            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Section)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

            if (enrollment is null)
                return null;

            return new EnrollmentDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                StudentName = enrollment.Student != null ? $"{enrollment.Student.FirstName} {enrollment.Student.LastName}".Trim() : string.Empty,
                StudentNumber = enrollment.Student?.StudentNumber ?? string.Empty,
                SectionId = enrollment.SectionId,
                SectionName = enrollment.Section?.Name ?? string.Empty,
                CourseName = enrollment.Section?.Course?.Name ?? string.Empty,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status,
                Grade = enrollment.Grade,
                GradePoints = enrollment.GradePoints
            };
        }
    }
}
