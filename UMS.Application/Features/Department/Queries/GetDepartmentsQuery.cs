
using MediatR;
using DepartmentEntity = UMS.Domain.Entities.Department;

namespace UMS.Application.Features.Department.Queries
{
    // Request to retrieve all departments.
    public class GetDepartmentsQuery : IRequest<IEnumerable<DepartmentEntity>>
    {
    }
}
