using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.Student.Commands;
using UMS.Application.Interfaces;
using StudentEntity = UMS.Domain.Entities.Student;

namespace UMS.Application.Features.Student.Handlers
{
    // Handles the request to update an existing student.
    public class UpdateStudentCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<UpdateStudentCommand, StudentEntity?>
    {
        public async Task<StudentEntity?> Handle(
            UpdateStudentCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.StudentDto;

            // Checks whether the student exists.
            var existingStudent = await dbContext.Students
                .FirstOrDefaultAsync(x => x.Id == request.StudentId, cancellationToken);

            if (existingStudent is null)
            {
                return null;
            }

            // Checks whether the student number belongs to another student.
            var numberExists = await dbContext.Students
                .AnyAsync(s => s.StudentNumber == dto.StudentNumber && s.Id != request.StudentId, cancellationToken);

            if (numberExists)
            {
                throw new InvalidOperationException(
                    $"Student number {dto.StudentNumber} already exists.");
            }

            // Checks whether the email belongs to another student.
            var emailExists = await dbContext.Students
                .AnyAsync(s => s.Email == dto.Email && s.Id != request.StudentId, cancellationToken);

            if (emailExists)
            {
                throw new InvalidOperationException(
                    $"Email {dto.Email} is already registered.");
            }

            // Checks whether the department exists.
            var departmentExists = await dbContext.Departments
                .AnyAsync(d => d.Id == dto.DepartmentId, cancellationToken);

            if (!departmentExists)
            {
                throw new InvalidOperationException(
                    $"Department {dto.DepartmentId} does not exist.");
            }

            // Updates the student fields.
            existingStudent.FirstName = dto.FirstName;
            existingStudent.LastName = dto.LastName;
            existingStudent.Email = dto.Email;
            existingStudent.StudentNumber = dto.StudentNumber;
            existingStudent.DateOfBirth = dto.DateOfBirth;
            existingStudent.PhoneNumber = dto.PhoneNumber;
            existingStudent.DepartmentId = dto.DepartmentId;

            await dbContext.SaveChangesAsync(cancellationToken);

            return existingStudent;
        }
    }
}
