using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Enrollment;
using UMS.Application.Features.Enrollment.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Enrollment.Handlers
{
    // Handles the command to create a new enrollment with comprehensive university validations.
    public class CreateEnrollmentCommandHandler : IRequestHandler<CreateEnrollmentCommand, EnrollmentDto>
    {
        private readonly IAppDbContext _context;

        public CreateEnrollmentCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<EnrollmentDto> Handle(
            CreateEnrollmentCommand request,
            CancellationToken cancellationToken)
        {
            var section = await _context.Sections
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken);

            if (section is null)
                throw new KeyNotFoundException("Section not found.");

            // 1. Section Capacity Limit Check
            var activeEnrolledCount = await _context.Enrollments
                .CountAsync(e => e.SectionId == request.SectionId && e.Status == "Enrolled", cancellationToken);

            if (activeEnrolledCount >= section.Capacity)
            {
                throw new InvalidOperationException(
                    $"Section '{section.Name}' is full. Capacity of {section.Capacity} students has been reached.");
            }

            // 2. Existing Enrollment & Re-Enrollment Check in this specific section
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == request.StudentId && e.SectionId == request.SectionId, cancellationToken);

            if (existingEnrollment != null)
            {
                if (string.Equals(existingEnrollment.Status?.Trim(), "Enrolled", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("This student is already actively enrolled in this section.");
                }

                if (string.Equals(existingEnrollment.Status?.Trim(), "Dropped", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "You have already dropped this course in the current semester and cannot re-enroll until next semester.");
                }
            }

            // 3. Duplicate Course in Semester Check (across any section in same semester)
            var duplicateCourseInSemester = await _context.Enrollments
                .Include(e => e.Section)
                .AnyAsync(e => e.StudentId == request.StudentId &&
                               e.Section.CourseId == section.CourseId &&
                               e.Section.SemesterId == section.SemesterId &&
                               e.Status == "Enrolled", cancellationToken);

            if (duplicateCourseInSemester)
            {
                var courseName = section.Course?.Name ?? "this course";
                throw new InvalidOperationException(
                    $"Student is already enrolled in course '{courseName}' for this semester.");
            }

            // 4. Student Schedule Clash Check
            var newSectionTimetables = await _context.Timetables
                .AsNoTracking()
                .Where(t => t.SectionId == request.SectionId)
                .ToListAsync(cancellationToken);

            if (newSectionTimetables.Count > 0)
            {
                var studentActiveSectionIds = await _context.Enrollments
                    .AsNoTracking()
                    .Where(e => e.StudentId == request.StudentId && e.Status == "Enrolled")
                    .Select(e => e.SectionId)
                    .ToListAsync(cancellationToken);

                if (studentActiveSectionIds.Count > 0)
                {
                    var existingSchedules = await _context.Timetables
                        .AsNoTracking()
                        .Where(t => studentActiveSectionIds.Contains(t.SectionId))
                        .Include(t => t.Section)
                            .ThenInclude(s => s.Course)
                        .ToListAsync(cancellationToken);

                    foreach (var newSlot in newSectionTimetables)
                    {
                        var clash = existingSchedules.FirstOrDefault(e =>
                            e.DayOfWeek == newSlot.DayOfWeek &&
                            newSlot.StartTime < e.EndTime && e.StartTime < newSlot.EndTime);

                        if (clash != null)
                        {
                            var clashCourseName = clash.Section?.Course?.Name ?? clash.Section?.Name ?? "another class";
                            throw new InvalidOperationException(
                                $"Schedule conflict: This class on {newSlot.DayOfWeek} ({newSlot.StartTime:hh\\:mm tt} - {newSlot.EndTime:hh\\:mm tt}) clashes with your existing enrolled class '{clashCourseName}'.");
                        }
                    }
                }
            }

            var enrollment = new UMS.Domain.Entities.Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = request.StudentId,
                SectionId = request.SectionId,
                EnrollmentDate = request.EnrollmentDate == default ? DateOnly.FromDateTime(DateTime.UtcNow) : request.EnrollmentDate,
                Status = "Enrolled"
            };

            _context.Enrollments.Add(enrollment);
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
