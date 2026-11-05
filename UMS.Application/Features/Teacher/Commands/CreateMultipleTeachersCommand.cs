using MediatR;
using UMS.Application.DTOs.Teacher;

namespace UMS.Application.Features.Teacher.Commands
{
    // Request to register multiple teachers.
    public class CreateMultipleTeachersCommand(List<CreateTeacherDto> teachers)
        : IRequest<(List<TeacherDto> AddedTeachers, List<object> RejectedTeachers)>
    {
        public List<CreateTeacherDto> Teachers { get; } = teachers;
    }
}
