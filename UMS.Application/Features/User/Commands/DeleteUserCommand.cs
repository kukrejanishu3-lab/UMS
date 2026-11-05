using MediatR;

namespace UMS.Application.Features.User.Commands
{
    // Request to delete a user by ID.
    public class DeleteUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }

        public DeleteUserCommand(Guid userId)
        {
            UserId = userId;
        }
    }
}