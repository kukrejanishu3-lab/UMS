
using MediatR;
using UMS.Application.DTOs.Student;
using StudentEntity = UMS.Domain.Entities.Student;

namespace UMS.Application.Features.Student.Commands
{
    // Request to create a new student.
    public class CreateStudentCommand : IRequest<StudentEntity>
    {
        public CreateStudentDto StudentDto { get; set; }

        public CreateStudentCommand(CreateStudentDto studentDto)
        {
            StudentDto = studentDto;
        }
    }
}
