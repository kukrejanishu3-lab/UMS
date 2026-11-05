using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Enrollment;
using UMS.Application.Features.Enrollment.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Enrollment.Handlers
{
    // Handles the command to update an existing enrollment.
    public class UpdateEnrollmentCommandHandler : IRequestHandler<UpdateEnrollmentCommand, EnrollmentDto?>
    {
        private readonly IAppDbContext _context;

        public UpdateEnrollmentCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<EnrollmentDto?> Handle(
            UpdateEnrollmentCommand request,
            CancellationToken cancellationToken)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

            if (enrollment is null)
                return null;

            enrollment.StudentId = request.StudentId;
            enrollment.SectionId = request.SectionId;
            enrollment.EnrollmentDate = request.EnrollmentDate;
            enrollment.Status = request.Status;

            await _context.SaveChangesAsync(cancellationToken);

            return new EnrollmentDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                SectionId = enrollment.SectionId,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status,
                Grade = enrollment.Grade,
                GradePoints = enrollment.GradePoints
            };
        }
    }
}
