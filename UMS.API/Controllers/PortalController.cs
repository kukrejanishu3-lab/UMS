using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.Portal;
using UMS.Application.Features.Attendance.Queries;
using UMS.Application.Features.Grading.Queries;
using UMS.Application.Features.Portal.Queries;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortalController(ISender sender) : ControllerBase
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

        // ==========================================
        // STUDENT PORTAL ENDPOINTS
        // ==========================================

        // Retrieves all active courses for the logged-in student.
        [HttpGet("student/my-courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentCourses()
        {
            var (email, userId) = GetCurrentUserInfo();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found in token." });

            try
            {
                var courses = await sender.Send(new GetStudentMyCoursesQuery(email, userId));
                return Ok(courses);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Retrieves the weekly class schedule for the logged-in student.
        [HttpGet("student/my-schedule")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentSchedule()
        {
            var (email, userId) = GetCurrentUserInfo();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found in token." });

            try
            {
                var schedule = await sender.Send(new GetStudentMyScheduleQuery(email, userId));
                return Ok(schedule);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Retrieves all course drop requests submitted by the logged-in student.
        [HttpGet("student/my-drop-requests")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentDropRequests()
        {
            var (email, userId) = GetCurrentUserInfo();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found in token." });

            try
            {
                var dropRequests = await sender.Send(new GetStudentMyDropRequestsQuery(email, userId));
                return Ok(dropRequests);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Retrieves course-wise attendance percentage and 75% short attendance alerts for the logged-in student.
        [HttpGet("student/my-attendance")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentAttendance()
        {
            var (email, userId) = GetCurrentUserInfo();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found in token." });

            try
            {
                var attendance = await sender.Send(new GetStudentAttendanceQuery(email, userId));
                return Ok(attendance);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Retrieves full academic transcript with SGPA and CGPA for the logged-in student.
        [HttpGet("student/my-transcript")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentTranscript()
        {
            var (email, userId) = GetCurrentUserInfo();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found in token." });

            try
            {
                var transcript = await sender.Send(new GetStudentTranscriptQuery(email, userId));
                return Ok(transcript);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ==========================================
        // TEACHER PORTAL ENDPOINTS
        // ==========================================

        // Retrieves all sections assigned to the logged-in teacher.
        [HttpGet("teacher/my-sections")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherSections()
        {
            var (email, userId) = GetCurrentUserInfo();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found in token." });

            try
            {
                var sections = await sender.Send(new GetTeacherMySectionsQuery(email, userId));
                return Ok(sections);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Retrieves the weekly teaching schedule for the logged-in teacher.
        [HttpGet("teacher/my-schedule")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherSchedule()
        {
            var (email, userId) = GetCurrentUserInfo();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found in token." });

            try
            {
                var schedule = await sender.Send(new GetTeacherMyScheduleQuery(email, userId));
                return Ok(schedule);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Retrieves the list of students in a specific section assigned to the teacher.
        [HttpGet("teacher/section/{sectionId:guid}/students")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherSectionStudents(Guid sectionId)
        {
            var (email, userId) = GetCurrentUserInfo();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found in token." });

            try
            {
                var students = await sender.Send(new GetTeacherSectionStudentsQuery(sectionId, email, userId));
                return Ok(students);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
