using MediatR;

namespace UMS.Application.Features.SectionTeacher.Commands
{
    // Command to remove a teacher from a section.
    public class RemoveTeacherCommand : IRequest<bool>
    {
        public Guid SectionId { get; set; }
        public Guid TeacherId { get; set; }
    }
}
