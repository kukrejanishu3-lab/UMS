using MediatR;
using UMS.Application.DTOs.Attendance;

namespace UMS.Application.Features.Attendance.Commands
{
    // Command to mark or update attendance for students in a section on a given date (and optional timetable slot)
    public record MarkAttendanceCommand(
        Guid SectionId,
        DateOnly Date,
        Guid? TimetableId,
        List<StudentAttendanceItemDto> Attendances,
        string TeacherEmail,
        Guid? TeacherUserId) : IRequest<MarkAttendanceResultDto>;
}
