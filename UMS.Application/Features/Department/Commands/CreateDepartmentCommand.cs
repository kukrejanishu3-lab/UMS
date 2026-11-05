
using MediatR;
using UMS.Application.DTOs.Department;
using DepartmentEntity = UMS.Domain.Entities.Department;

namespace UMS.Application.Features.Department.Commands
{
    // Request to create a new department.
    public class CreateDepartmentCommand : IRequest<DepartmentEntity>
    {
        public CreateDepartmentDto DepartmentDto { get; set; }

        public CreateDepartmentCommand(CreateDepartmentDto departmentDto)
        {
            DepartmentDto = departmentDto;
        }
    }
}
