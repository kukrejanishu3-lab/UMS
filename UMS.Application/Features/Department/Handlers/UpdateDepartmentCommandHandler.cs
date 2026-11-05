using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.Department.Commands;
using UMS.Application.Interfaces;
using DepartmentEntity = UMS.Domain.Entities.Department;

namespace UMS.Application.Features.Department.Handlers
{
    // Handles the request to update an existing department.
    public class UpdateDepartmentCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<UpdateDepartmentCommand, DepartmentEntity?>
    {
        public async Task<DepartmentEntity?> Handle(
            UpdateDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var existingDepartment = await dbContext.Departments
                .FirstOrDefaultAsync(x => x.Id == request.DepartmentId, cancellationToken);

            if (existingDepartment is null)
            {
                return null;
            }

            existingDepartment.Name = request.DepartmentDto.Name;
            existingDepartment.Code = request.DepartmentDto.Code;

            await dbContext.SaveChangesAsync(cancellationToken);

            return existingDepartment;
        }
    }
}
