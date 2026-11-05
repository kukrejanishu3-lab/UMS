using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Enrollment;
using UMS.Application.Features.Enrollment.Commands;
using UMS.Application.Features.Enrollment.Queries;
using UMS.Application.Interfaces;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController(IMediator mediator, IAppDbContext context) : ControllerBase
    {
        // Gets all enrollments.
        [HttpGet]
        public async Task<IActionResult> GetEnrollments()
        {
            var enrollments = await mediator.Send(new GetEnrollmentsQuery());
            return Ok(enrollments);
        }

        // Gets an enrollment by ID.
        [HttpGet("{enrollmentId:guid}")]
        public async Task<IActionResult> GetEnrollmentById(Guid enrollmentId)
        {
            var enrollment = await mediator.Send(
                new GetEnrollmentByIdQuery { EnrollmentId = enrollmentId });

            if (enrollment is null)
                return NotFound(new { message = "Enrollment not found." });

            return Ok(enrollment);
        }

        // Creates an enrollment after validating the student and section. Only Admin can enroll.
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AddEnrollment([FromBody] CreateEnrollmentDto dto)
        {
            var studentExists = await context.Students
                .AnyAsync(s => s.Id == dto.StudentId);

            if (!studentExists)
                return BadRequest(new { message = "Student not found." });

            var sectionExists = await context.Sections
                .AnyAsync(s => s.Id == dto.SectionId);

            if (!sectionExists)
                return BadRequest(new { message = "Section not found." });


            try
            {
                var command = new CreateEnrollmentCommand
                {
                    StudentId = dto.StudentId,
                    SectionId = dto.SectionId,
                    EnrollmentDate = dto.EnrollmentDate ?? DateOnly.FromDateTime(DateTime.UtcNow)
                };

                var created = await mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetEnrollmentById),
                    new { enrollmentId = created.Id },
                    created);
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

        // Updates an existing enrollment.
        [HttpPut("{enrollmentId:guid}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateEnrollment(
            Guid enrollmentId,
            [FromBody] UpdateEnrollmentDto dto)
        {
            var studentExists = await context.Students
                .AnyAsync(s => s.Id == dto.StudentId);

            if (!studentExists)
                return BadRequest(new { message = "Student not found." });

            var sectionExists = await context.Sections
                .AnyAsync(s => s.Id == dto.SectionId);

            if (!sectionExists)
                return BadRequest(new { message = "Section not found." });

            var duplicateEnrollment = await context.Enrollments
                .AnyAsync(e =>
                    e.Id != enrollmentId &&
                    e.StudentId == dto.StudentId &&
                    e.SectionId == dto.SectionId);

            if (duplicateEnrollment)
                return Conflict(new { message = "This student is already enrolled in this section." });

            var command = new UpdateEnrollmentCommand
            {
                EnrollmentId = enrollmentId,
                StudentId = dto.StudentId,
                SectionId = dto.SectionId,
                EnrollmentDate = dto.EnrollmentDate,
                Status = dto.Status
            };

            var updated = await mediator.Send(command);

            if (updated is null)
                return NotFound(new { message = "Enrollment not found." });

            return Ok(updated);
        }

        // Deletes an enrollment by ID.
        [HttpDelete("{enrollmentId:guid}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteEnrollment(Guid enrollmentId)
        {
            var deleted = await mediator.Send(
                new DeleteEnrollmentCommand { EnrollmentId = enrollmentId });

            if (!deleted)
                return NotFound(new { message = "Enrollment not found." });

            return NoContent();
        }
    }
}
