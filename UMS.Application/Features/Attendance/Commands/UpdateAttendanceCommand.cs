using MediatR;
using UMS.Application.DTOs.Attendance;

namespace UMS.Application.Features.Attendance.Commands
{
    // Command to update a specific student's attendance record
    public record UpdateAttendanceCommand(
        Guid AttendanceId,
        UpdateAttendanceDto Dto,
        string UserEmail,
        Guid? UserId) : IRequest<StudentAttendanceRecordDto>;
}
