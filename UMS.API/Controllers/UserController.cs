using MediatR;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.User;
using UMS.Application.Features.User.Commands;
using UMS.Application.Features.User.Queries;
namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(
    ISender sender) : ControllerBase
    {
        // Retrieves all users
        // Retrieves all users through MediatR.
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await sender.Send(new GetUsersQuery());

            return Ok(users);
        }

        // Retrieves a user by ID
        // Retrieves a user by ID through MediatR.
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await sender.Send(new GetUserByIdQuery(userId));

            if (user is null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }


        // Creates a new user
        // Creates a new user through MediatR.
        [HttpPost]
        public async Task<IActionResult> AddUser(
            [FromBody] CreateUserDto userDto)
        {
            try
            {
                var createdUser = await sender.Send(
                    new CreateUserCommand(userDto));

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { userId = createdUser.Id },
                    createdUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


        // Updates an existing user
        // Updates an existing user through MediatR.
        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateUser(
            Guid userId,
            [FromBody] UpdateUserDto userDto)
        {
            try
            {
                var updatedUser = await sender.Send(
                    new UpdateUserCommand(userId, userDto));

                if (updatedUser is null)
                    return NotFound(new { message = "User not found." });

                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


        // Deletes a user through MediatR.
        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var deleted = await sender.Send(
                new DeleteUserCommand(userId));

            if (!deleted)
                return NotFound(new { message = "User not found." });

            return NoContent();
        }
    }
}
