using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.DropCourseRequest;
using UMS.Application.Features.DropCourseRequest.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.DropCourseRequest.Handlers
{
    // Handles the command to submit a new drop-course request.
    public class CreateDropCourseRequestCommandHandler
        : IRequestHandler<CreateDropCourseRequestCommand, DropCourseRequestDto>
    {
        private readonly IAppDbContext _context;

        public CreateDropCourseRequestCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<DropCourseRequestDto> Handle(
            CreateDropCourseRequestCommand request,
            CancellationToken cancellationToken)
        {
            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Section)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

            var dropRequest = new UMS.Domain.Entities.DropCourseRequest
            {
                Id = Guid.NewGuid(),
                EnrollmentId = request.EnrollmentId,
                Reason = request.Reason,
                Status = "Pending",
                RequestDate = DateTime.UtcNow,
                AdminRemarks = null
            };

            _context.DropCourseRequests.Add(dropRequest);
            await _context.SaveChangesAsync(cancellationToken);

            return new DropCourseRequestDto
            {
                Id = dropRequest.Id,
                EnrollmentId = dropRequest.EnrollmentId,
                StudentName = enrollment?.Student != null ? $"{enrollment.Student.FirstName} {enrollment.Student.LastName}".Trim() : string.Empty,
                StudentNumber = enrollment?.Student?.StudentNumber ?? string.Empty,
                CourseName = enrollment?.Section?.Course?.Name ?? string.Empty,
                SectionName = enrollment?.Section?.Name ?? string.Empty,
                Reason = dropRequest.Reason,
                Status = dropRequest.Status,
                RequestDate = DateOnly.FromDateTime(dropRequest.RequestDate),
                AdminRemarks = dropRequest.AdminRemarks
            };
        }
    }
}
