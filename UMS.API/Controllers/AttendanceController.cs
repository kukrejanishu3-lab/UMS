using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.Attendance;
using UMS.Application.Features.Attendance.Commands;
using UMS.Application.Features.Attendance.Queries;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController(ISender sender) : ControllerBase
    {
        private (string Email, Guid? UserId) GetCurrentUserInfo()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value
                ?? string.Empty;

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            Guid? userId = Guid.TryParse(userIdStr, out var id) ? id : null;

            return (email, userId);
        }

        // Submits or updates bulk attendance for all students in a section on a specific date.
        [HttpPost("section/{sectionId:guid}")]
        [Authorize(Roles = "Teacher,SuperAdmin")]
        public async Task<IActionResult> MarkAttendance(Guid sectionId, [FromBody] MarkAttendanceDto dto)
        {
            var (email, userId) = GetCurrentUserInfo();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found in token." });

            try
            {
                var result = await sender.Send(new MarkAttendanceCommand(
                    sectionId, dto.Date, dto.TimetableId, dto.Attendances, email, userId));
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Retrieves the attendance sheet of a section for a specific date (and optional timetable slot).
        [HttpGet("section/{sectionId:guid}")]
        [Authorize(Roles = "Teacher,SuperAdmin")]
        public async Task<IActionResult> GetSectionAttendance(Guid sectionId, [FromQuery] DateOnly date, [FromQuery] Guid? timetableId = null)
        {
            try
            {
                var sheet = await sender.Send(new GetSectionAttendanceByDateQuery(sectionId, date, timetableId));
                return Ok(sheet);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Updates a single student's attendance record (e.g. correcting Absent to Present or adding remarks)
        [HttpPut("{attendanceId:guid}")]
        [Authorize(Roles = "Teacher,SuperAdmin")]
        public async Task<IActionResult> UpdateAttendance(Guid attendanceId, [FromBody] UpdateAttendanceDto dto)
        {
            var (email, userId) = GetCurrentUserInfo();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found in token." });

            try
            {
                var updated = await sender.Send(new UpdateAttendanceCommand(attendanceId, dto, email, userId));
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
