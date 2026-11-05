using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.Department.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Department.Handlers
{
    // Handles the request to delete a department.
    public class DeleteDepartmentCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<DeleteDepartmentCommand, bool>
    {
        public async Task<bool> Handle(
            DeleteDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var department = await dbContext.Departments
                .FirstOrDefaultAsync(x => x.Id == request.DepartmentId, cancellationToken);

            if (department is null)
            {
                return false;
            }

            dbContext.Departments.Remove(department);
            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}