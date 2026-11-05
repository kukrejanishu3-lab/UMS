using MediatR;
using UMS.Application.DTOs.Section;

namespace UMS.Application.Features.Section.Queries
{
    // Query to get all sections.
    public class GetSectionsQuery : IRequest<List<SectionDto>>
    {
    }
}
