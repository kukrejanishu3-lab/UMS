using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.DropCourseRequest;
using UMS.Application.Features.DropCourseRequest.Commands;
using UMS.Application.Features.DropCourseRequest.Queries;
using UMS.Application.Interfaces;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DropCourseRequestController(
        IMediator mediator,
        IAppDbContext context) : ControllerBase
    {
        // Gets all drop-course requests.
        [HttpGet]
        public async Task<IActionResult> GetDropCourseRequests()
        {
            var requests = await mediator.Send(new GetDropCourseRequestsQuery());
            return Ok(requests);
        }

        // Gets a drop-course request by ID.
        [HttpGet("{requestId:guid}")]
        public async Task<IActionResult> GetDropCourseRequestById(Guid requestId)
        {
            var request = await mediator.Send(
                new GetDropCourseRequestByIdQuery { RequestId = requestId });

            if (request is null)
                return NotFound(new { message = "Drop-course request not found." });

            return Ok(request);
        }

        // Submits a drop-course request for an existing enrollment.
        [HttpPost]
        public async Task<IActionResult> AddDropCourseRequest(
            [FromBody] CreateDropCourseRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Reason))
                return BadRequest(new { message = "A reason is required." });

            var enrollment = await context.Enrollments
                .FirstOrDefaultAsync(e => e.Id == dto.EnrollmentId);

            if (enrollment is null)
                return NotFound(new { message = "Enrollment not found." });

            if (string.Equals(enrollment.Status, "Dropped", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "This course is already dropped. You cannot submit a drop request for an inactive enrollment." });

            var pendingRequestExists = await context.DropCourseRequests
                .AnyAsync(r => r.EnrollmentId == dto.EnrollmentId && r.Status == "Pending");

            if (pendingRequestExists)
                return Conflict(new
                {
                    message = "A pending drop-course request already exists for this enrollment."
                });

            var command = new CreateDropCourseRequestCommand
            {
                EnrollmentId = dto.EnrollmentId,
                Reason = dto.Reason
            };

            var created = await mediator.Send(command);

            return CreatedAtAction(
                nameof(GetDropCourseRequestById),
                new { requestId = created.Id },
                created);
        }

        // Approves or rejects a pending drop-course request. Only Admin can approve/reject.
        [HttpPut("{requestId:guid}/status")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateRequestStatus(
            Guid requestId,
            [FromBody] UpdateDropCourseRequestStatusDto dto)
        {
            // Validate the existing request state before sending to handler.
            var existing = await context.DropCourseRequests
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (existing is null)
                return NotFound(new { message = "Drop-course request not found." });

            if (existing.Status != "Pending")
                return Conflict(new { message = "Only pending requests can be approved or rejected." });

            if (dto.Status != "Approved" && dto.Status != "Rejected")
                return BadRequest(new { message = "Status must be Approved or Rejected." });

            var command = new UpdateDropCourseRequestStatusCommand
            {
                RequestId = requestId,
                Status = dto.Status,
                AdminRemarks = dto.AdminRemarks
            };

            var updated = await mediator.Send(command);
            return Ok(updated);
        }

        // Deletes a drop-course request by ID.
        [HttpDelete("{requestId:guid}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteDropCourseRequest(Guid requestId)
        {
            var deleted = await mediator.Send(
                new DeleteDropCourseRequestCommand { RequestId = requestId });

            if (!deleted)
                return NotFound(new { message = "Drop-course request not found." });

            return NoContent();
        }
    }

    public class UpdateDropCourseRequestStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? AdminRemarks { get; set; }
    }
}
