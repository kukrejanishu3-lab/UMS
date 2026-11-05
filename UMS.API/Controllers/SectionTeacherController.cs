using MediatR;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.SectionTeacher;
using UMS.Application.Features.SectionTeacher.Commands;
using UMS.Application.Features.SectionTeacher.Queries;

namespace UMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionTeacherController(IMediator mediator) : ControllerBase
    {
        // Retrieves all teacher-section assignments.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assignments = await mediator.Send(new GetAllSectionTeachersQuery());
            return Ok(assignments);
        }

        // Retrieves teachers assigned to a specific section.
        [HttpGet("section/{sectionId:guid}")]
        public async Task<IActionResult> GetBySectionId(Guid sectionId)
        {
            var assignments = await mediator.Send(
                new GetTeachersBySectionIdQuery { SectionId = sectionId });

            if (assignments is null)
                return NotFound("Section not found.");

            return Ok(assignments);
        }

        // Assigns an existing teacher to an existing section.
        [HttpPost]
        public async Task<IActionResult> AssignTeacher(AssignTeacherDto dto)
        {
            var result = await mediator.Send(new AssignTeacherCommand
            {
                SectionId = dto.SectionId,
                TeacherId = dto.TeacherId
            });

            return result switch
            {
                "SectionNotFound" => NotFound("Section not found."),
                "TeacherNotFound" => NotFound("Teacher not found."),
                "AlreadyAssigned" => Conflict("This teacher is already assigned to this section."),
                _ => Ok("Teacher assigned to section successfully.")
            };
        }

        // Removes a teacher from a section.
        [HttpDelete("{sectionId:guid}/{teacherId:guid}")]
        public async Task<IActionResult> RemoveTeacher(Guid sectionId, Guid teacherId)
        {
            var deleted = await mediator.Send(new RemoveTeacherCommand
            {
                SectionId = sectionId,
                TeacherId = teacherId
            });

            if (!deleted)
                return NotFound("Teacher assignment not found.");

            return Ok("Teacher removed from section successfully.");
        }
    }
}
