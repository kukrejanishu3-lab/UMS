using MediatR;
using UMS.Application.Features.Department.Commands;
using UMS.Application.Interfaces;
using DepartmentEntity = UMS.Domain.Entities.Department;

namespace UMS.Application.Features.Department.Handlers
{
    // Handles the request to create a new department.
    public class CreateDepartmentCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<CreateDepartmentCommand, DepartmentEntity>
    {
        public async Task<DepartmentEntity> Handle(
            CreateDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var department = new DepartmentEntity
            {
                Id = Guid.NewGuid(),
                Name = request.DepartmentDto.Name,
                Code = request.DepartmentDto.Code
            };

            await dbContext.Departments.AddAsync(department, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return department;
        }
    }
}
