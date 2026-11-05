using MediatR;
using UMS.Application.DTOs.Teacher;

namespace UMS.Application.Features.Teacher.Commands
{
    // Request to register a new teacher.
    public class CreateTeacherCommand(CreateTeacherDto teacherDto) : IRequest<TeacherDto>
    {
        public CreateTeacherDto TeacherDto { get; } = teacherDto;
    }
}
