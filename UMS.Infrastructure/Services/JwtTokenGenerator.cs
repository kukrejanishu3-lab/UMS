using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UMS.Application.Interfaces;
using UMS.Domain.Entities;

namespace UMS.Infrastructure.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, DateTime ExpiresAt) GenerateToken(User user)
        {
            var keyString = _configuration["Jwt:Key"]
                ?? "YourSuperSecretKeyForUMSApplication2026!SecureKeyMustBeLongEnough";
            var issuer = _configuration["Jwt:Issuer"] ?? "UMS.API";
            var audience = _configuration["Jwt:Audience"] ?? "UMS.Client";
            var expiryInHours = int.TryParse(_configuration["Jwt:ExpiryInHours"], out var hours) ? hours : 24;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role),
                new("role", user.Role)
            };

            var expiresAt = DateTime.UtcNow.AddHours(expiryInHours);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (tokenString, expiresAt);
        }
    }
}
