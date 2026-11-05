using MediatR;
using UMS.Application.DTOs.User;

namespace UMS.Application.Features.User.Commands
{
    // Request to update an existing user.
    public class UpdateUserCommand : IRequest<UserDto?>
    {
        public Guid UserId { get; set; }
        public UpdateUserDto UserDto { get; set; }

        public UpdateUserCommand(Guid userId, UpdateUserDto userDto)
        {
            UserId = userId;
            UserDto = userDto;
        }
    }
}