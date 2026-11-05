using MediatR;
using UMS.Application.DTOs.Section;

namespace UMS.Application.Features.Section.Queries
{
    // Query to get a single section by its ID.
    public class GetSectionByIdQuery : IRequest<SectionDto?>
    {
        public Guid SectionId { get; set; }
    }
}
