using MediatR;
using UMS.Application.DTOs.Portal;

namespace UMS.Application.Features.Portal.Queries
{
    // Queries for Student Portal
    public record GetStudentMyCoursesQuery(string UserEmail, Guid? UserId) : IRequest<List<StudentCourseDto>>;

    public record GetStudentMyScheduleQuery(string UserEmail, Guid? UserId) : IRequest<List<StudentScheduleDto>>;

    public record GetStudentMyDropRequestsQuery(string UserEmail, Guid? UserId) : IRequest<List<StudentDropRequestDto>>;

    // Queries for Teacher Portal
    public record GetTeacherMySectionsQuery(string UserEmail, Guid? UserId) : IRequest<List<TeacherSectionDto>>;

    public record GetTeacherMyScheduleQuery(string UserEmail, Guid? UserId) : IRequest<List<TeacherScheduleDto>>;

    public record GetTeacherSectionStudentsQuery(Guid SectionId, string UserEmail, Guid? UserId) : IRequest<List<SectionStudentRosterDto>>;
}
