using MediatR;
using UMS.Application.DTOs.Teacher;

namespace UMS.Application.Features.Teacher.Commands
{
    // Request to update an existing teacher.
    public class UpdateTeacherCommand(Guid teacherId, UpdateTeacherDto teacherDto) : IRequest<TeacherDto?>
    {
        public Guid TeacherId { get; } = teacherId;
        public UpdateTeacherDto TeacherDto { get; } = teacherDto;
    }
}
