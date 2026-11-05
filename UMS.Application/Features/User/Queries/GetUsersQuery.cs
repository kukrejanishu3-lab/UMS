using MediatR;
using UMS.Application.DTOs.User;

namespace UMS.Application.Features.User.Queries
{
    // Request to retrieve all users.
    public class GetUsersQuery : IRequest<IEnumerable<UserDto>>
    {
    }
}