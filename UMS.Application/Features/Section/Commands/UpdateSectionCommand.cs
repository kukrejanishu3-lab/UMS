using MediatR;
using UMS.Application.DTOs.Section;

namespace UMS.Application.Features.Section.Commands
{
    // Command to update an existing section.
    public class UpdateSectionCommand : IRequest<SectionDto?>
    {
        public Guid SectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public Guid CourseId { get; set; }
        public Guid SemesterId { get; set; }
    }
}
