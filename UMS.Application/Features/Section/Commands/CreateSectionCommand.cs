using MediatR;
using UMS.Application.DTOs.Section;

namespace UMS.Application.Features.Section.Commands
{
    // Command to create a single new section.
    public class CreateSectionCommand : IRequest<SectionDto>
    {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public Guid CourseId { get; set; }
        public Guid SemesterId { get; set; }
    }
}
