using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.User;
using UMS.Application.Features.User.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.User.Handlers
{
    // Handles the request to retrieve one user by ID.
    public class GetUserByIdQueryHandler(IAppDbContext dbContext)
        : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        public async Task<UserDto?> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == request.UserId)
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}