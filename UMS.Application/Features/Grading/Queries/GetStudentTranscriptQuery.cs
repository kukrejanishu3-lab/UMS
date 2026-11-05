using MediatR;
using UMS.Application.DTOs.Grading;

namespace UMS.Application.Features.Grading.Queries
{
    // Query to retrieve a student's full academic transcript with SGPA and CGPA
    public record GetStudentTranscriptQuery(
        string? UserEmail = null,
        Guid? UserId = null,
        Guid? StudentId = null) : IRequest<StudentTranscriptDto>;
}
