using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.User;
using UMS.Application.Features.User.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.User.Handlers
{
    // Handles the request to update an existing user.
    public class UpdateUserCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<UpdateUserCommand, UserDto?>
    {
        public async Task<UserDto?> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            var userDto = request.UserDto;

            // Validate email.
            if (string.IsNullOrWhiteSpace(userDto.Email))
                throw new ArgumentException("Email is required.");

            // Validate the requested role.
            var allowedRoles = new[] { "SuperAdmin", "Student", "Teacher" };

            var role = allowedRoles.FirstOrDefault(
                r => r.Equals(
                    userDto.Role,
                    StringComparison.OrdinalIgnoreCase));

            if (role is null)
                throw new ArgumentException("Invalid user role.");

            var email = userDto.Email.Trim();

            // Check whether the user exists.
            var existingUser = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (existingUser is null)
                return null;

            // Check whether another user already uses this email.
            var emailExists = await dbContext.Users
                .AnyAsync(u => u.Email == email && u.Id != request.UserId, cancellationToken);

            if (emailExists)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            existingUser.Email = email;
            existingUser.Role = role;

            await dbContext.SaveChangesAsync(cancellationToken);

            // Return safe user details without the password hash.
            return new UserDto
            {
                Id = existingUser.Id,
                Email = existingUser.Email,
                Role = existingUser.Role
            };
        }
    }
}