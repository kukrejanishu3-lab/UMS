
using MediatR;
using UMS.Application.DTOs.Department;
using DepartmentEntity = UMS.Domain.Entities.Department;

namespace UMS.Application.Features.Department.Commands
{
    // Request to update an existing department.
    public class UpdateDepartmentCommand : IRequest<DepartmentEntity?>
    {
        public Guid DepartmentId { get; set; }

        public CreateDepartmentDto DepartmentDto { get; set; }

        public UpdateDepartmentCommand(
            Guid departmentId,
            CreateDepartmentDto departmentDto)
        {
            DepartmentId = departmentId;
            DepartmentDto = departmentDto;
        }
    }
}
