using MediatR;

namespace UMS.Application.Features.Section.Commands
{
    // Command to delete a section by its ID.
    public class DeleteSectionCommand : IRequest<bool>
    {
        public Guid SectionId { get; set; }
    }
}
