using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Attendance;
using UMS.Application.Features.Attendance.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Attendance.Handlers
{
    public class GetStudentAttendanceQueryHandler : IRequestHandler<GetStudentAttendanceQuery, List<StudentCourseAttendanceDto>>
    {
        private readonly IAppDbContext _context;

        public GetStudentAttendanceQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentCourseAttendanceDto>> Handle(GetStudentAttendanceQuery request, CancellationToken cancellationToken)
        {
            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == request.UserEmail, cancellationToken);

            if (student is null)
            {
                throw new KeyNotFoundException($"Student profile not found for '{request.UserEmail}'.");
            }

            // Get all courses currently or previously enrolled
            var enrollments = await _context.Enrollments
                .Include(e => e.Section)
                    .ThenInclude(s => s.Course)
                .AsNoTracking()
                .Where(e => e.StudentId == student.Id)
                .ToListAsync(cancellationToken);

            var result = new List<StudentCourseAttendanceDto>();

            foreach (var enrollment in enrollments)
            {
                // Total lectures / class slots conducted in this section
                var totalLecturesHeld = await _context.Attendances
                    .AsNoTracking()
                    .Where(a => a.SectionId == enrollment.SectionId)
                    .Select(a => new { a.Date, a.TimetableId })
                    .Distinct()
                    .CountAsync(cancellationToken);

                // Student's records for this section
                var studentAttendances = await _context.Attendances
                    .AsNoTracking()
                    .Where(a => a.SectionId == enrollment.SectionId && a.StudentId == student.Id)
                    .ToListAsync(cancellationToken);

                var attendedCount = studentAttendances.Count(a =>
                    string.Equals(a.Status, "Present", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(a.Status, "Late", StringComparison.OrdinalIgnoreCase));

                var absentCount = studentAttendances.Count(a =>
                    string.Equals(a.Status, "Absent", StringComparison.OrdinalIgnoreCase));

                decimal percentage = totalLecturesHeld > 0
                    ? Math.Round(((decimal)attendedCount / totalLecturesHeld) * 100m, 2)
                    : 100m;

                bool isShort = totalLecturesHeld > 0 && percentage < 75.0m;

                string statusMessage = totalLecturesHeld == 0
                    ? "No attendance records recorded yet."
                    : isShort
                        ? $"WARNING: Attendance is {percentage}% (below 75% HEC exam eligibility criteria)."
                        : $"Eligible: Attendance is {percentage}%.";

                result.Add(new StudentCourseAttendanceDto
                {
                    CourseId = enrollment.Section?.CourseId ?? Guid.Empty,
                    CourseName = enrollment.Section?.Course?.Name ?? "N/A",
                    CourseCode = enrollment.Section?.Course?.Code ?? "N/A",
                    SectionId = enrollment.SectionId,
                    SectionName = enrollment.Section?.Name ?? "N/A",
                    TotalLectures = totalLecturesHeld,
                    AttendedLectures = attendedCount,
                    AbsentLectures = absentCount,
                    AttendancePercentage = percentage,
                    IsShortAttendance = isShort,
                    StatusMessage = statusMessage
                });
            }

            return result;
        }
    }
}
