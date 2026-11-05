using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.Grading;
using UMS.Application.Features.Grading.Commands;
using UMS.Application.Features.Grading.Queries;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GradingController(ISender sender) : ControllerBase
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

        // Submits or updates course evaluation marks for students in a section (Auto HEC Grade calculation)
        [HttpPost("section/{sectionId:guid}")]
        [Authorize(Roles = "Teacher,SuperAdmin")]
        public async Task<IActionResult> SubmitGrades(Guid sectionId, [FromBody] SubmitCourseGradesDto dto)
        {
            var (email, userId) = GetCurrentUserInfo();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found in token." });

            try
            {
                var result = await sender.Send(new SubmitCourseGradesCommand(
                    sectionId, dto.Grades, email, userId));
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Retrieves the awards list / grade sheet of a section
        [HttpGet("section/{sectionId:guid}")]
        [Authorize(Roles = "Teacher,SuperAdmin")]
        public async Task<IActionResult> GetSectionGrades(Guid sectionId)
        {
            var (email, userId) = GetCurrentUserInfo();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found in token." });

            try
            {
                var sheet = await sender.Send(new GetSectionGradesQuery(sectionId, email, userId));
                return Ok(sheet);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Updates an individual student's course marks (e.g. after paper re-checking or recounting)
        [HttpPut("enrollment/{enrollmentId:guid}")]
        [Authorize(Roles = "Teacher,SuperAdmin")]
        public async Task<IActionResult> UpdateGrade(Guid enrollmentId, [FromBody] UpdateGradeDto dto)
        {
            var (email, userId) = GetCurrentUserInfo();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found in token." });

            try
            {
                var result = await sender.Send(new UpdateStudentGradeCommand(
                    enrollmentId, dto, email, userId));
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Retrieves full academic transcript for a specific student
        [HttpGet("student/{studentId:guid}/transcript")]
        [Authorize(Roles = "Teacher,SuperAdmin")]
        public async Task<IActionResult> GetStudentTranscript(Guid studentId)
        {
            try
            {
                var transcript = await sender.Send(new GetStudentTranscriptQuery(null, null, studentId));
                return Ok(transcript);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
