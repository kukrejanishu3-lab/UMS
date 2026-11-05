using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.Section.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Section.Handlers
{
    // Handles the command to delete a section by its ID.
    public class DeleteSectionCommandHandler : IRequestHandler<DeleteSectionCommand, bool>
    {
        private readonly IAppDbContext _context;

        public DeleteSectionCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(
            DeleteSectionCommand request,
            CancellationToken cancellationToken)
        {
            var section = await _context.Sections
                .FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken);

            if (section is null)
                return false;

            _context.Sections.Remove(section);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
