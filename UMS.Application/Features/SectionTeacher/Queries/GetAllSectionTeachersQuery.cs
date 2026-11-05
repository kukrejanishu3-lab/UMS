using MediatR;

namespace UMS.Application.Features.SectionTeacher.Queries
{
    public class SectionTeacherDto
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
    }

    // Query to get all teacher-section assignments.
    public class GetAllSectionTeachersQuery : IRequest<List<SectionTeacherDto>>
    {
    }
}
