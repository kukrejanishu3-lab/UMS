using MediatR;
using UMS.Application.DTOs.Grading;

namespace UMS.Application.Features.Grading.Commands
{
    // Command for a teacher/admin to submit course evaluation marks for students in a section
    public record SubmitCourseGradesCommand(
        Guid SectionId,
        List<StudentGradeInputDto> Grades,
        string UserEmail,
        Guid? UserId) : IRequest<SubmitCourseGradesResultDto>;
}
