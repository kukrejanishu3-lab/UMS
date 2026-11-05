using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.DropCourseRequest;
using UMS.Application.Features.DropCourseRequest.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.DropCourseRequest.Handlers
{
    // Handles the query to retrieve all drop-course requests with student and course details.
    public class GetDropCourseRequestsQueryHandler
        : IRequestHandler<GetDropCourseRequestsQuery, List<DropCourseRequestDto>>
    {
        private readonly IAppDbContext _context;

        public GetDropCourseRequestsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DropCourseRequestDto>> Handle(
            GetDropCourseRequestsQuery request,
            CancellationToken cancellationToken)
        {
            var requests = await _context.DropCourseRequests
                .AsNoTracking()
                .Include(r => r.Enrollment)
                    .ThenInclude(e => e.Student)
                .Include(r => r.Enrollment)
                    .ThenInclude(e => e.Section)
                        .ThenInclude(s => s.Course)
                .ToListAsync(cancellationToken);

            return requests.Select(r => new DropCourseRequestDto
            {
                Id = r.Id,
                EnrollmentId = r.EnrollmentId,
                StudentName = r.Enrollment?.Student != null ? $"{r.Enrollment.Student.FirstName} {r.Enrollment.Student.LastName}".Trim() : string.Empty,
                StudentNumber = r.Enrollment?.Student?.StudentNumber ?? string.Empty,
                CourseName = r.Enrollment?.Section?.Course?.Name ?? string.Empty,
                SectionName = r.Enrollment?.Section?.Name ?? string.Empty,
                Reason = r.Reason,
                Status = r.Status,
                RequestDate = DateOnly.FromDateTime(r.RequestDate),
                AdminRemarks = r.AdminRemarks
            }).ToList();
        }
    }
}
