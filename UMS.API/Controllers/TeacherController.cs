using MediatR;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.Teacher;
using UMS.Application.Features.Teacher.Commands;
using UMS.Application.Features.Teacher.Queries;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController(ISender sender) : ControllerBase
    {
        // Gets all teachers.
        [HttpGet]
        public async Task<IActionResult> GetTeachers()
        {
            var teachers = await sender.Send(new GetTeachersQuery());
            return Ok(teachers);
        }

        // Gets a teacher by ID.
        [HttpGet("{teacherId:guid}")]
        public async Task<IActionResult> GetTeacherById(Guid teacherId)
        {
            var teacher = await sender.Send(new GetTeacherByIdQuery(teacherId));

            if (teacher is null)
            {
                return NotFound(new { message = "Teacher not found." });
            }

            return Ok(teacher);
        }

        // Registers a teacher if their employee number is not already used.
        [HttpPost]
        public async Task<IActionResult> AddTeacher([FromBody] CreateTeacherDto dto)
        {
            try
            {
                var createdTeacher = await sender.Send(new CreateTeacherCommand(dto));

                return CreatedAtAction(
                    nameof(GetTeacherById),
                    new { teacherId = createdTeacher.Id },
                    createdTeacher);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Adds valid teachers and skips duplicate employee numbers.
        [HttpPost("multiple")]
        public async Task<IActionResult> AddTeachers([FromBody] List<CreateTeacherDto> teachers)
        {
            if (teachers is null || teachers.Count == 0)
            {
                return BadRequest(new { message = "At least one teacher is required." });
            }

            var result = await sender.Send(new CreateMultipleTeachersCommand(teachers));

            return Ok(new
            {
                message = "Multiple teacher registration completed.",
                addedCount = result.AddedTeachers.Count,
                rejectedCount = result.RejectedTeachers.Count,
                addedTeachers = result.AddedTeachers,
                rejectedTeachers = result.RejectedTeachers
            });
        }

        // Updates a teacher if the employee number is not used by another teacher.
        [HttpPut("{teacherId:guid}")]
        public async Task<IActionResult> UpdateTeacher(
            Guid teacherId,
            [FromBody] UpdateTeacherDto dto)
        {
            try
            {
                var updatedTeacher = await sender.Send(new UpdateTeacherCommand(teacherId, dto));

                if (updatedTeacher is null)
                {
                    return NotFound(new { message = "Teacher not found." });
                }

                return Ok(updatedTeacher);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Deletes a teacher by ID.
        [HttpDelete("{teacherId:guid}")]
        public async Task<IActionResult> DeleteTeacher(Guid teacherId)
        {
            var deleted = await sender.Send(new DeleteTeacherCommand(teacherId));

            if (!deleted)
            {
                return NotFound(new { message = "Teacher not found." });
            }

            return NoContent();
        }
    }
}
