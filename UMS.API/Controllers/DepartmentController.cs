
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.Department;
using UMS.Application.Features.Department.Commands;
using UMS.Application.Features.Department.Queries;

namespace UMS.API.Controllers.DepartmentControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController(ISender sender) : ControllerBase
    {
        // Returns all departments.
        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await sender.Send(new GetDepartmentsQuery());
            return Ok(departments);
        }

        // Returns a department by its ID.
        [HttpGet("{departmentId:guid}")]
        public async Task<IActionResult> GetDepartmentById(Guid departmentId)
        {
            var department = await sender.Send(
                new GetDepartmentByIdQuery(departmentId));

            if (department == null)
                return NotFound();

            return Ok(department);
        }

        // Creates a new department.
        [HttpPost]
        public async Task<IActionResult> AddDepartment(
            [FromBody] CreateDepartmentDto dto)
        {
            var department = await sender.Send(
                new CreateDepartmentCommand(dto));

            return CreatedAtAction(
                nameof(GetDepartmentById),
                new { departmentId = department.Id },
                department);
        }

        // Updates an existing department.
        [HttpPut("{departmentId:guid}")]
        public async Task<IActionResult> UpdateDepartment(
            Guid departmentId,
            [FromBody] CreateDepartmentDto dto)
        {
            var department = await sender.Send(
                new UpdateDepartmentCommand(departmentId, dto));

            if (department == null)
                return NotFound();

            return Ok(department);
        }

        // Deletes a department by its ID.
        [HttpDelete("{departmentId:guid}")]
        public async Task<IActionResult> DeleteDepartment(Guid departmentId)
        {
            var deleted = await sender.Send(
                new DeleteDepartmentCommand(departmentId));

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
