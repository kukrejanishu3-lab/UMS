using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.User;
using UMS.Application.Features.User.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.User.Handlers
{
    // Handles the request to retrieve all users.
    public class GetUsersQueryHandler(IAppDbContext dbContext)
        : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
    {
        public async Task<IEnumerable<UserDto>> Handle(
            GetUsersQuery request,
            CancellationToken cancellationToken)
        {
            return await dbContext.Users
                .AsNoTracking()
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role
                })
                .ToListAsync(cancellationToken);
        }
    }
}
