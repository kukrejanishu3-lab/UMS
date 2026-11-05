using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.User;
using UMS.Application.Features.User.Commands;
using UMS.Application.Interfaces;
using UserEntity = UMS.Domain.Entities.User;

namespace UMS.Application.Features.User.Handlers
{
    // Handles the request to create a new user.
    public class CreateUserCommandHandler(
        IAppDbContext dbContext,
        IPasswordHasher<UserEntity> passwordHasher)
        : IRequestHandler<CreateUserCommand, UserDto>
    {
        public async Task<UserDto> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken)
        {
            var userDto = request.UserDto;

            // Validate required fields.
            if (string.IsNullOrWhiteSpace(userDto.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(userDto.Password))
                throw new ArgumentException("Password is required.");

            // Only these roles are currently allowed.
            var allowedRoles = new[] { "SuperAdmin", "Student", "Teacher" };

            var role = allowedRoles.FirstOrDefault(
                r => r.Equals(userDto.Role, StringComparison.OrdinalIgnoreCase));

            if (role is null)
                throw new ArgumentException("Invalid user role.");

            var email = userDto.Email.Trim();

            // Check whether the email is already registered.
            var emailExists = await dbContext.Users
                .AnyAsync(u => u.Email == email, cancellationToken);

            if (emailExists)
                throw new InvalidOperationException("Email already exists.");

            // Create the user entity.
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = email,
                Role = role
            };

            // Hash the password before saving it.
            user.PasswordHash = passwordHasher.HashPassword(
                user, userDto.Password);

            await dbContext.Users.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Return safe user details without the password hash.
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
