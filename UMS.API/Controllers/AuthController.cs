using MediatR;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.Auth;
using UMS.Application.Features.Auth.Commands;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(ISender sender) : ControllerBase
    {
        // Authenticates a user and returns a JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            try
            {
                var response = await sender.Send(new LoginCommand(loginDto));
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
