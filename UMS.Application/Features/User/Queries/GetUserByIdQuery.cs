using MediatR;
using UMS.Application.DTOs.User;

namespace UMS.Application.Features.User.Queries
{
    // Request to retrieve one user by ID.
    public class GetUserByIdQuery : IRequest<UserDto?>
    {
        public Guid UserId { get; set; }

        public GetUserByIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}