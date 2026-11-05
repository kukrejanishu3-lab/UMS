using MediatR;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.Section;
using UMS.Application.Features.Section.Commands;
using UMS.Application.Features.Section.Queries;

namespace UMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController(IMediator mediator) : ControllerBase
    {
        // Retrieves all sections.
        [HttpGet]
        public async Task<IActionResult> GetSections()
        {
            var sections = await mediator.Send(new GetSectionsQuery());
            return Ok(sections);
        }

        // Retrieves a section by its ID.
        [HttpGet("{sectionId:guid}")]
        public async Task<IActionResult> GetSectionById(Guid sectionId)
        {
            var section = await mediator.Send(new GetSectionByIdQuery { SectionId = sectionId });

            if (section is null)
                return NotFound("Section not found.");

            return Ok(section);
        }

        // Adds a single section.
        [HttpPost]
        public async Task<IActionResult> AddSection(CreateSectionDto dto)
        {
            var command = new CreateSectionCommand
            {
                Name = dto.Name,
                Capacity = dto.Capacity,
                CourseId = dto.CourseId,
                SemesterId = dto.SemesterId
            };

            var createdSection = await mediator.Send(command);

            return CreatedAtAction(
                nameof(GetSectionById),
                new { sectionId = createdSection.Id },
                createdSection);
        }

        // Adds multiple sections and skips duplicates for the same course.
        [HttpPost("multiple")]
        public async Task<IActionResult> AddSections([FromBody] List<CreateSectionDto> sections)
        {
            var command = new CreateMultipleSectionsCommand { Sections = sections };
            var result = await mediator.Send(command);
            return Ok(result);
        }

        // Updates an existing section.
        [HttpPut("{sectionId:guid}")]
        public async Task<IActionResult> UpdateSection(Guid sectionId, UpdateSectionDto dto)
        {
            var command = new UpdateSectionCommand
            {
                SectionId = sectionId,
                Name = dto.Name,
                Capacity = dto.Capacity,
                CourseId = dto.CourseId,
                SemesterId = dto.SemesterId
            };

            var updatedSection = await mediator.Send(command);

            if (updatedSection is null)
                return NotFound("Section not found.");

            return Ok(updatedSection);
        }

        // Deletes a section by its ID.
        [HttpDelete("{sectionId:guid}")]
        public async Task<IActionResult> DeleteSection(Guid sectionId)
        {
            var deleted = await mediator.Send(new DeleteSectionCommand { SectionId = sectionId });

            if (!deleted)
                return NotFound("Section not found.");

            return Ok("Section deleted successfully.");
        }
    }
}