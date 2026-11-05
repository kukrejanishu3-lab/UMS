
using MediatR;
using UMS.Application.DTOs.Student;
using StudentEntity = UMS.Domain.Entities.Student;

namespace UMS.Application.Features.Student.Commands
{
    // Request to register multiple students.
    public class CreateMultipleStudentsCommand
        : IRequest<(List<StudentEntity> AddedStudents, List<object> RejectedStudents)>
    {
        public List<CreateStudentDto> Students { get; set; }

        public CreateMultipleStudentsCommand(List<CreateStudentDto> students)
        {
            Students = students;
        }
    }
}
