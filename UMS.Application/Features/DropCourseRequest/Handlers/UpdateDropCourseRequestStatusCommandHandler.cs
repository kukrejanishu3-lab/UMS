using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.DropCourseRequest;
using UMS.Application.Features.DropCourseRequest.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.DropCourseRequest.Handlers
{
    // Handles the command to approve or reject a pending drop-course request.
    public class UpdateDropCourseRequestStatusCommandHandler
        : IRequestHandler<UpdateDropCourseRequestStatusCommand, DropCourseRequestDto?>
    {
        private readonly IAppDbContext _context;

        public UpdateDropCourseRequestStatusCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<DropCourseRequestDto?> Handle(
            UpdateDropCourseRequestStatusCommand request,
            CancellationToken cancellationToken)
        {
            var dropRequest = await _context.DropCourseRequests
                .Include(r => r.Enrollment)
                    .ThenInclude(e => e.Student)
                .Include(r => r.Enrollment)
                    .ThenInclude(e => e.Section)
                        .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

            if (dropRequest is null)
                return null;

            dropRequest.Status = request.Status;
            dropRequest.AdminRemarks = request.AdminRemarks;

            // When a drop request is approved, update the corresponding enrollment status to Dropped and Grade to W (Withdrawn).
            if (string.Equals(request.Status, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                if (dropRequest.Enrollment is not null)
                {
                    dropRequest.Enrollment.Status = "Dropped";
                    dropRequest.Enrollment.Grade = "W";
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new DropCourseRequestDto
            {
                Id = dropRequest.Id,
                EnrollmentId = dropRequest.EnrollmentId,
                StudentName = dropRequest.Enrollment?.Student != null ? $"{dropRequest.Enrollment.Student.FirstName} {dropRequest.Enrollment.Student.LastName}".Trim() : string.Empty,
                StudentNumber = dropRequest.Enrollment?.Student?.StudentNumber ?? string.Empty,
                CourseName = dropRequest.Enrollment?.Section?.Course?.Name ?? string.Empty,
                SectionName = dropRequest.Enrollment?.Section?.Name ?? string.Empty,
                Reason = dropRequest.Reason,
                Status = dropRequest.Status,
                RequestDate = DateOnly.FromDateTime(dropRequest.RequestDate),
                AdminRemarks = dropRequest.AdminRemarks
            };
        }
    }
}
