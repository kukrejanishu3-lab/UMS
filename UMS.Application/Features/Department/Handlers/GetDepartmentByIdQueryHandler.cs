using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.Department.Queries;
using UMS.Application.Interfaces;
using DepartmentEntity = UMS.Domain.Entities.Department;

namespace UMS.Application.Features.Department.Handlers
{
    // Handles the request to retrieve a department by its ID.
    public class GetDepartmentByIdQueryHandler(IAppDbContext dbContext)
        : IRequestHandler<GetDepartmentByIdQuery, DepartmentEntity?>
    {
        public async Task<DepartmentEntity?> Handle(
            GetDepartmentByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await dbContext.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.DepartmentId, cancellationToken);
        }
    }
}
