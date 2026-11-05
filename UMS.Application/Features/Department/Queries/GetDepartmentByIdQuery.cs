
using MediatR;
using DepartmentEntity = UMS.Domain.Entities.Department;

namespace UMS.Application.Features.Department.Queries
{
    // Request to retrieve a department by its ID.
    public class GetDepartmentByIdQuery : IRequest<DepartmentEntity?>
    {
        public Guid DepartmentId { get; set; }

        public GetDepartmentByIdQuery(Guid departmentId)
        {
            DepartmentId = departmentId;
        }
    }
}
