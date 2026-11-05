using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Attendance;
using UMS.Application.Features.Attendance.Commands;
using UMS.Application.Interfaces;
using UMS.Domain.Entities;

namespace UMS.Application.Features.Attendance.Handlers
{
    public class MarkAttendanceCommandHandler : IRequestHandler<MarkAttendanceCommand, MarkAttendanceResultDto>
    {
        private readonly IAppDbContext _context;

        public MarkAttendanceCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<MarkAttendanceResultDto> Handle(MarkAttendanceCommand request, CancellationToken cancellationToken)
        {
            // 1. Verify User identity (Teacher or Admin)
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => (request.TeacherUserId.HasValue && u.Id == request.TeacherUserId) || u.Email == request.TeacherEmail, cancellationToken);

            bool isAdmin = user != null && (user.Role == "Admin" || user.Role == "SuperAdmin");

            var teacher = await _context.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == request.TeacherEmail, cancellationToken);

            if (!isAdmin && teacher is null)
            {
                throw new KeyNotFoundException($"Teacher profile not found for '{request.TeacherEmail}'.");
            }

            // 2. Verify Section exists
            var section = await _context.Sections
                .Include(s => s.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken);

            if (section is null)
            {
                throw new KeyNotFoundException("Section not found.");
            }

            // 3. If teacher (not admin), verify Teacher is assigned to this Section (via SectionTeacher or Timetable)
            if (!isAdmin && teacher is not null)
            {
                var isAssigned = await _context.SectionTeachers
                    .AnyAsync(st => st.SectionId == request.SectionId && st.TeacherId == teacher.Id, cancellationToken)
                    || await _context.Timetables
                    .AnyAsync(t => t.SectionId == request.SectionId && t.TeacherId == teacher.Id, cancellationToken);

                if (!isAssigned)
                {
                    throw new InvalidOperationException($"Teacher conflict: You are not assigned to teach section '{section.Name}' and cannot mark attendance for it.");
                }
            }

            // 3b. Verify Timetable slot if specified
            string? timeSlotStr = null;
            if (request.TimetableId.HasValue)
            {
                var timetable = await _context.Timetables
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == request.TimetableId.Value && t.SectionId == request.SectionId, cancellationToken);

                if (timetable is null)
                {
                    throw new KeyNotFoundException($"Timetable slot '{request.TimetableId.Value}' was not found for section '{section.Name}'.");
                }

                timeSlotStr = $"{timetable.StartTime:hh\\:mm} - {timetable.EndTime:hh\\:mm} ({timetable.DayOfWeek})";
            }

            // 4. Fetch Active Enrolled Students in this Section
            var activeEnrollments = await _context.Enrollments
                .Include(e => e.Student)
                .AsNoTracking()
                .Where(e => e.SectionId == request.SectionId && e.Status == "Enrolled")
                .ToListAsync(cancellationToken);

            var activeStudentDict = activeEnrollments.ToDictionary(e => e.StudentId, e => e.Student);

            // Filter incoming attendances to only those students actively enrolled
            var validAttendances = request.Attendances
                .Where(a => activeStudentDict.ContainsKey(a.StudentId))
                .ToList();

            if (!validAttendances.Any())
            {
                throw new InvalidOperationException("No enrolled students found in this section to mark attendance for.");
            }

            // 5. Fetch existing attendance records for this section, date, and timetable slot (to support seamless update/upsert)
            var existingAttendances = await _context.Attendances
                .Where(a => a.SectionId == request.SectionId && a.Date == request.Date && a.TimetableId == request.TimetableId)
                .ToListAsync(cancellationToken);

            int presentCount = 0;
            int absentCount = 0;
            int lateCount = 0;
            int excusedCount = 0;

            var resultRecords = new List<StudentAttendanceRecordDto>();

            foreach (var item in validAttendances)
            {
                var student = activeStudentDict[item.StudentId];

                // Normalize status (default to Present if invalid)
                var status = item.Status.Trim();
                if (!string.Equals(status, "Present", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(status, "Absent", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(status, "Late", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(status, "Excused", StringComparison.OrdinalIgnoreCase))
                {
                    status = "Present";
                }

                if (string.Equals(status, "Present", StringComparison.OrdinalIgnoreCase)) presentCount++;
                else if (string.Equals(status, "Absent", StringComparison.OrdinalIgnoreCase)) absentCount++;
                else if (string.Equals(status, "Late", StringComparison.OrdinalIgnoreCase)) lateCount++;
                else if (string.Equals(status, "Excused", StringComparison.OrdinalIgnoreCase)) excusedCount++;

                var existing = existingAttendances.FirstOrDefault(a => a.StudentId == item.StudentId);
                Guid attendanceId;
                if (existing is not null)
                {
                    // Update existing attendance
                    existing.Status = status;
                    existing.Remarks = item.Remarks;
                    attendanceId = existing.Id;
                }
                else
                {
                    attendanceId = Guid.NewGuid();
                    // Insert new attendance
                    _context.Attendances.Add(new Domain.Entities.Attendance
                    {
                        Id = attendanceId,
                        SectionId = request.SectionId,
                        StudentId = item.StudentId,
                        Date = request.Date,
                        TimetableId = request.TimetableId,
                        Status = status,
                        Remarks = item.Remarks
                    });
                }

                resultRecords.Add(new StudentAttendanceRecordDto
                {
                    AttendanceId = attendanceId,
                    StudentId = student.Id,
                    StudentRollNo = student.StudentNumber,
                    StudentName = $"{student.FirstName} {student.LastName}".Trim(),
                    TimetableId = request.TimetableId,
                    TimeSlot = timeSlotStr,
                    Status = status,
                    Remarks = item.Remarks
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new MarkAttendanceResultDto
            {
                SectionId = section.Id,
                SectionName = section.Name,
                CourseCode = section.Course?.Code ?? "N/A",
                CourseName = section.Course?.Name ?? "N/A",
                Date = request.Date,
                TimetableId = request.TimetableId,
                TimeSlot = timeSlotStr,
                TotalMarked = validAttendances.Count,
                PresentCount = presentCount,
                AbsentCount = absentCount,
                LateCount = lateCount,
                ExcusedCount = excusedCount,
                Message = $"Attendance marked successfully for '{section.Course?.Name}' ({section.Name}) on {request.Date:yyyy-MM-dd}{(timeSlotStr != null ? $" [{timeSlotStr}]" : "")}.",
                Records = resultRecords.OrderBy(r => r.StudentRollNo).ToList()
            };
        }
    }
}
