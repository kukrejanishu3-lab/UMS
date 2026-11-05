using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Auth;
using UMS.Application.Features.Auth.Commands;
using UMS.Application.Interfaces;
using UserEntity = UMS.Domain.Entities.User;

namespace UMS.Application.Features.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IAppDbContext _dbContext;
        private readonly IPasswordHasher<UserEntity> _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginCommandHandler(
            IAppDbContext dbContext,
            IPasswordHasher<UserEntity> passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var loginDto = request.LoginDto;

            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                throw new ArgumentException("Email and password are required.");
            }

            var email = loginDto.Email.Trim();

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash, loginDto.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = expiresAt
            };
        }
    }
}
