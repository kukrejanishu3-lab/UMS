using MediatR;
using UMS.Application.DTOs.Attendance;

namespace UMS.Application.Features.Attendance.Queries
{
    // Query to retrieve attendance records for a section on a specific date (and optional timetable slot)
    public record GetSectionAttendanceByDateQuery(
        Guid SectionId,
        DateOnly Date,
        Guid? TimetableId = null) : IRequest<SectionAttendanceSheetDto>;
}
