using MediatR;
using UMS.Application.DTOs.Auth;

namespace UMS.Application.Features.Auth.Commands
{
    public record LoginCommand(LoginRequestDto LoginDto) : IRequest<AuthResponseDto>;
}
