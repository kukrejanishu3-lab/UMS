using MediatR;
using UMS.Application.DTOs.Section;

namespace UMS.Application.Features.Section.Commands
{
    // Command to create multiple sections at once, skipping duplicates.
    public class CreateMultipleSectionsCommand : IRequest<CreateMultipleSectionsResultDto>
    {
        public List<CreateSectionDto> Sections { get; set; } = new();
    }

    public class CreateMultipleSectionsResultDto
    {
        public string Message { get; set; } = string.Empty;
        public int AddedCount { get; set; }
        public int RejectedCount { get; set; }
        public List<SectionDto> AddedSections { get; set; } = new();
        public List<object> RejectedSections { get; set; } = new();
    }
}
