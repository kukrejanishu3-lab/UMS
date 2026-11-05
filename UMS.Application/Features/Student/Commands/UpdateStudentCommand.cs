
using MediatR;
using UMS.Application.DTOs.Student;
using StudentEntity = UMS.Domain.Entities.Student;

namespace UMS.Application.Features.Student.Commands
{
    // Request to update an existing student.
    public class UpdateStudentCommand : IRequest<StudentEntity?>
    {
        public Guid StudentId { get; set; }

        public CreateStudentDto StudentDto { get; set; }

        public UpdateStudentCommand(
            Guid studentId,
            CreateStudentDto studentDto)
        {
            StudentId = studentId;
            StudentDto = studentDto;
        }
    }
}
