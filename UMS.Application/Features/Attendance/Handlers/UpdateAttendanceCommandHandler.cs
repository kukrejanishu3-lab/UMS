using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Attendance;
using UMS.Application.Features.Attendance.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Attendance.Handlers
{
    public class UpdateAttendanceCommandHandler : IRequestHandler<UpdateAttendanceCommand, StudentAttendanceRecordDto>
    {
        private readonly IAppDbContext _context;

        public UpdateAttendanceCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<StudentAttendanceRecordDto> Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Section)
                .Include(a => a.Timetable)
                .FirstOrDefaultAsync(a => a.Id == request.AttendanceId, cancellationToken);

            if (attendance is null)
            {
                throw new KeyNotFoundException("Attendance record not found.");
            }

            // Check if user is a teacher: verify they are assigned to this section
            var teacher = await _context.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == request.UserEmail, cancellationToken);

            if (teacher is not null)
            {
                var isAssigned = await _context.SectionTeachers
                    .AnyAsync(st => st.SectionId == attendance.SectionId && st.TeacherId == teacher.Id, cancellationToken)
                    || await _context.Timetables
                    .AnyAsync(t => t.SectionId == attendance.SectionId && t.TeacherId == teacher.Id, cancellationToken);

                if (!isAssigned)
                {
                    throw new InvalidOperationException($"Teacher conflict: You are not assigned to section '{attendance.Section.Name}' and cannot update its attendance.");
                }
            }

            // Normalize status
            var status = request.Dto.Status.Trim();
            if (!string.Equals(status, "Present", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(status, "Absent", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(status, "Late", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(status, "Excused", StringComparison.OrdinalIgnoreCase))
            {
                status = "Present";
            }

            attendance.Status = status;
            attendance.Remarks = request.Dto.Remarks;

            await _context.SaveChangesAsync(cancellationToken);

            string? timeSlotStr = attendance.Timetable != null
                ? $"{attendance.Timetable.StartTime:hh\\:mm} - {attendance.Timetable.EndTime:hh\\:mm}"
                : null;

            return new StudentAttendanceRecordDto
            {
                AttendanceId = attendance.Id,
                StudentId = attendance.StudentId,
                StudentRollNo = attendance.Student.StudentNumber,
                StudentName = $"{attendance.Student.FirstName} {attendance.Student.LastName}".Trim(),
                TimetableId = attendance.TimetableId,
                TimeSlot = timeSlotStr,
                Status = attendance.Status,
                Remarks = attendance.Remarks
            };
        }
    }
}
