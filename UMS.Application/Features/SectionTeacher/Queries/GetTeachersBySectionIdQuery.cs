using MediatR;

namespace UMS.Application.Features.SectionTeacher.Queries
{
    public class SectionTeacherBySectionDto
    {
        public Guid SectionId { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
    }

    // Query to get all teacher assignments for a specific section.
    public class GetTeachersBySectionIdQuery : IRequest<List<SectionTeacherBySectionDto>?>
    {
        public Guid SectionId { get; set; }
    }
}
