using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.Student.Commands;
using UMS.Application.Interfaces;
using StudentEntity = UMS.Domain.Entities.Student;

namespace UMS.Application.Features.Student.Handlers
{
    // Handles the request to create a new student.
    public class CreateStudentCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<CreateStudentCommand, StudentEntity>
    {
        public async Task<StudentEntity> Handle(
            CreateStudentCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.StudentDto;

            // Checks whether the student number already exists.
            var numberExists = await dbContext.Students
                .AnyAsync(s => s.StudentNumber == dto.StudentNumber, cancellationToken);

            if (numberExists)
            {
                throw new InvalidOperationException(
                    $"Student number {dto.StudentNumber} already exists.");
            }

            // Checks whether the email already exists.
            var emailExists = await dbContext.Students
                .AnyAsync(s => s.Email == dto.Email, cancellationToken);

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

            // Maps the DTO to the Student entity.
            var student = new StudentEntity
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                StudentNumber = dto.StudentNumber,
                DateOfBirth = dto.DateOfBirth,
                PhoneNumber = dto.PhoneNumber,
                DepartmentId = dto.DepartmentId,
                IsActive = true
            };

            await dbContext.Students.AddAsync(student, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return student;
        }
    }
}
