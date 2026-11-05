using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Timetable;
using UMS.Application.Features.Timetable.Commands;
using UMS.Application.Features.Timetable.Queries;
using UMS.Application.Interfaces;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimetableController(
        IMediator mediator,
        IAppDbContext dbContext) : ControllerBase
    {
        // Gets all timetable entries.
        [HttpGet]
        public async Task<IActionResult> GetTimetables()
        {
            var timetables = await mediator.Send(new GetTimetablesQuery());
            return Ok(timetables);
        }

        // Gets a timetable entry by ID.
        [HttpGet("{timetableId:guid}")]
        public async Task<IActionResult> GetTimetableById(Guid timetableId)
        {
            var timetable = await mediator.Send(new GetTimetableByIdQuery(timetableId));

            if (timetable is null)
            {
                return NotFound(new { message = "Timetable entry not found." });
            }

            return Ok(timetable);
        }

        // Creates a timetable entry after validating its section, teacher, and time. Only Admin can schedule.
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AddTimetable([FromBody] CreateTimetableDto dto)
        {
            if (dto.EndTime <= dto.StartTime)
            {
                return BadRequest(new
                {
                    message = "End time must be later than start time."
                });
            }

            var sectionExists = await dbContext.Sections
                .AnyAsync(s => s.Id == dto.SectionId);

            if (!sectionExists)
            {
                return BadRequest(new { message = "Section not found." });
            }

            var teacherExists = await dbContext.Teachers
                .AnyAsync(t => t.Id == dto.TeacherId);

            if (!teacherExists)
            {
                return BadRequest(new { message = "Teacher not found." });
            }

            try
            {
                var command = new CreateTimetableCommand
                {
                    SectionId = dto.SectionId,
                    TeacherId = dto.TeacherId,
                    DayOfWeek = dto.DayOfWeek,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    RoomNumber = dto.RoomNumber
                };

                var createdTimetable = await mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetTimetableById),
                    new { timetableId = createdTimetable.Id },
                    createdTimetable);
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

        // Updates an existing timetable entry.
        [HttpPut("{timetableId:guid}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateTimetable(
            Guid timetableId,
            [FromBody] UpdateTimetableDto dto)
        {
            if (dto.EndTime <= dto.StartTime)
            {
                return BadRequest(new
                {
                    message = "End time must be later than start time."
                });
            }

            var sectionExists = await dbContext.Sections
                .AnyAsync(s => s.Id == dto.SectionId);

            if (!sectionExists)
            {
                return BadRequest(new { message = "Section not found." });
            }

            var teacherExists = await dbContext.Teachers
                .AnyAsync(t => t.Id == dto.TeacherId);

            if (!teacherExists)
            {
                return BadRequest(new { message = "Teacher not found." });
            }

            try
            {
                var command = new UpdateTimetableCommand
                {
                    TimetableId = timetableId,
                    SectionId = dto.SectionId,
                    TeacherId = dto.TeacherId,
                    DayOfWeek = dto.DayOfWeek,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    RoomNumber = dto.RoomNumber
                };

                var updatedTimetable = await mediator.Send(command);

                if (updatedTimetable is null)
                {
                    return NotFound(new { message = "Timetable entry not found." });
                }

                return Ok(updatedTimetable);
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

        // Deletes a timetable entry by ID.
        [HttpDelete("{timetableId:guid}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteTimetable(Guid timetableId)
        {
            var deleted = await mediator.Send(
                new DeleteTimetableCommand { TimetableId = timetableId });

            if (!deleted)
            {
                return NotFound(new { message = "Timetable entry not found." });
            }

            return NoContent();
        }
    }
}
