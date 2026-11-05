
using MediatR;

namespace UMS.Application.Features.Department.Commands
{
    // Request to delete a department by its ID.
    public class DeleteDepartmentCommand : IRequest<bool>
    {
        public Guid DepartmentId { get; set; }

        public DeleteDepartmentCommand(Guid departmentId)
        {
            DepartmentId = departmentId;
        }
    }
}
