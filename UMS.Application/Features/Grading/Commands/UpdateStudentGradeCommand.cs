using MediatR;
using UMS.Application.DTOs.Grading;

namespace UMS.Application.Features.Grading.Commands
{
    // Command to update an individual student's course grade
    public record UpdateStudentGradeCommand(
        Guid EnrollmentId,
        UpdateGradeDto Dto,
        string UserEmail,
        Guid? UserId) : IRequest<StudentGradeRecordDto>;
}
