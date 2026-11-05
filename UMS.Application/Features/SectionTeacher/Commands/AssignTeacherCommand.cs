using MediatR;

namespace UMS.Application.Features.SectionTeacher.Commands
{
    // Command to assign a teacher to a section.
    public class AssignTeacherCommand : IRequest<string>
    {
        public Guid SectionId { get; set; }
        public Guid TeacherId { get; set; }
    }
}
