using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.User.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.User.Handlers
{
    // Handles the request to delete a user.
    public class DeleteUserCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<DeleteUserCommand, bool>
    {
        public async Task<bool> Handle(
            DeleteUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                return false;
            }

            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}