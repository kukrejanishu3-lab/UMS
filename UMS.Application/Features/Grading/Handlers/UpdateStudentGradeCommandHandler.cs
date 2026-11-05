using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Grading;
using UMS.Application.Features.Grading.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Grading.Handlers
{
    public class UpdateStudentGradeCommandHandler : IRequestHandler<UpdateStudentGradeCommand, StudentGradeRecordDto>
    {
        private readonly IAppDbContext _context;

        public UpdateStudentGradeCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<StudentGradeRecordDto> Handle(UpdateStudentGradeCommand request, CancellationToken cancellationToken)
        {
            // 1. Verify caller identity (Teacher or Admin)
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => (request.UserId.HasValue && u.Id == request.UserId) || u.Email == request.UserEmail, cancellationToken);

            bool isAdmin = user != null && (user.Role == "Admin" || user.Role == "SuperAdmin");

            var teacher = await _context.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == request.UserEmail, cancellationToken);

            if (!isAdmin && teacher is null)
            {
                throw new KeyNotFoundException($"Teacher profile not found for '{request.UserEmail}'.");
            }

            // 2. Fetch enrollment
            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Section)
                .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

            if (enrollment is null)
            {
                throw new KeyNotFoundException("Enrollment record not found.");
            }

            // 3. Verify Teacher assignment (if not admin)
            if (!isAdmin && teacher is not null)
            {
                var isAssigned = await _context.SectionTeachers
                    .AnyAsync(st => st.SectionId == enrollment.SectionId && st.TeacherId == teacher.Id, cancellationToken)
                    || await _context.Timetables
                    .AnyAsync(t => t.SectionId == enrollment.SectionId && t.TeacherId == teacher.Id, cancellationToken);

                if (!isAssigned)
                {
                    throw new InvalidOperationException($"Teacher conflict: You are not assigned to section '{enrollment.Section?.Name}' and cannot update its grades.");
                }
            }

            // 4. Validate dropped status
            if (string.Equals(enrollment.Status, "Dropped", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(enrollment.Grade, "W", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Cannot update marks for student '{enrollment.Student.StudentNumber}': The course was dropped and grade is locked as 'W'.");
            }

            // 5. Validate marks range
            if (request.Dto.Marks < 0 || request.Dto.Marks > 100)
            {
                throw new ArgumentException("Marks must be between 0 and 100.");
            }

            // 6. Recalculate HEC grade and grade points
            var (letterGrade, gradePoints) = SubmitCourseGradesCommandHandler.CalculateHecGrade(request.Dto.Marks);

            enrollment.Marks = request.Dto.Marks;
            enrollment.Grade = letterGrade;
            enrollment.GradePoints = gradePoints;

            await _context.SaveChangesAsync(cancellationToken);

            return new StudentGradeRecordDto
            {
                EnrollmentId = enrollment.Id,
                StudentId = enrollment.StudentId,
                StudentRollNo = enrollment.Student.StudentNumber,
                StudentName = $"{enrollment.Student.FirstName} {enrollment.Student.LastName}".Trim(),
                Marks = enrollment.Marks,
                Grade = enrollment.Grade,
                GradePoints = enrollment.GradePoints,
                Remarks = request.Dto.Remarks
            };
        }
    }
}
