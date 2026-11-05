using MediatR;
using UMS.Application.DTOs.Attendance;

namespace UMS.Application.Features.Attendance.Queries
{
    // Query to retrieve course-wise attendance breakdown and 75% rule for a student
    public record GetStudentAttendanceQuery(
        string UserEmail,
        Guid? UserId) : IRequest<List<StudentCourseAttendanceDto>>;
}
