using MediatR;
using UMS.Application.DTOs.User;

namespace UMS.Application.Features.User.Commands
{
    // Request to create a new user.
    public class CreateUserCommand : IRequest<UserDto>
    {
        public CreateUserDto UserDto { get; set; }

        public CreateUserCommand(CreateUserDto userDto)
        {
            UserDto = userDto;
        }
    }
}