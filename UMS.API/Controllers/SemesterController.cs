using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.Semester;
using UMS.Application.Features.Semester.Commands;
using UMS.Application.Features.Semester.Queries;

namespace UMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemesterController(ISender sender) : ControllerBase
    {
        // Retrieves all semesters.
        [HttpGet]
        public async Task<IActionResult> GetSemesters()
        {
            var semesters = await sender.Send(new GetSemestersQuery());
            return Ok(semesters);
        }

        // Retrieves a semester by its ID.
        [HttpGet("{semesterId:guid}")]
        public async Task<IActionResult> GetSemesterById(Guid semesterId)
        {
            var semester = await sender.Send(new GetSemesterByIdQuery(semesterId));

            if (semester is null)
            {
                return NotFound("Semester not found.");
            }

            return Ok(semester);
        }

        // Creates a new semester.
        [HttpPost]
        public async Task<IActionResult> AddSemester([FromBody] CreateSemesterDto dto)
        {
            var createdSemester = await sender.Send(new CreateSemesterCommand(dto));

            return CreatedAtAction(
                nameof(GetSemesterById),
                new { semesterId = createdSemester.Id },
                createdSemester);
        }

        // Updates an existing semester.
        [HttpPut("{semesterId:guid}")]
        public async Task<IActionResult> UpdateSemester(
            Guid semesterId,
            [FromBody] UpdateSemesterDto dto)
        {
            var updatedSemester = await sender.Send(new UpdateSemesterCommand(semesterId, dto));

            if (updatedSemester is null)
            {
                return NotFound("Semester not found.");
            }

            return Ok(updatedSemester);
        }

        // Deletes a semester by its ID.
        [HttpDelete("{semesterId:guid}")]
        public async Task<IActionResult> DeleteSemester(Guid semesterId)
        {
            var deleted = await sender.Send(new DeleteSemesterCommand(semesterId));

            if (!deleted)
            {
                return NotFound("Semester not found.");
            }

            return NoContent();
        }

        // Closes an active semester and evaluates / promotes students (HEC CGPA rule, probation warning, repeater flags)
        [HttpPost("{semesterId:guid}/close-and-promote")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CloseAndPromoteSemester(Guid semesterId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value
                ?? string.Empty;

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            Guid? userId = Guid.TryParse(userIdStr, out var id) ? id : null;

            try
            {
                var result = await sender.Send(new CloseAndPromoteSemesterCommand(semesterId, email, userId));
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
    }
}
