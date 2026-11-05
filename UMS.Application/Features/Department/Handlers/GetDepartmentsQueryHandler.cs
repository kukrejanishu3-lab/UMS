using MediatR;
using Microsoft.EntityFrameworkCore;
using DepartmentEntity = UMS.Domain.Entities.Department;
using UMS.Application.Features.Department.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Department.Handlers
{
    // Handles the request to retrieve all departments.
    public class GetDepartmentsQueryHandler(IAppDbContext dbContext)
        : IRequestHandler<GetDepartmentsQuery, IEnumerable<DepartmentEntity>>
    {
        public async Task<IEnumerable<DepartmentEntity>> Handle(
            GetDepartmentsQuery request,
            CancellationToken cancellationToken)
        {
            return await dbContext.Departments
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
