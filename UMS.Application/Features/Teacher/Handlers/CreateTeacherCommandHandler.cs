using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Teacher;
using UMS.Application.Features.Teacher.Commands;
using UMS.Application.Interfaces;
using TeacherEntity = UMS.Domain.Entities.Teacher;

namespace UMS.Application.Features.Teacher.Handlers
{
    // Handles creating a new teacher.
    public class CreateTeacherCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<CreateTeacherCommand, TeacherDto>
    {
        public async Task<TeacherDto> Handle(
            CreateTeacherCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.TeacherDto;

            // Checks whether the employee number already exists.
            var employeeNumberExists = await dbContext.Teachers
                .AnyAsync(t => t.EmployeeNumber == dto.EmployeeNumber, cancellationToken);

            if (employeeNumberExists)
            {
                throw new InvalidOperationException(
                    $"Employee number {dto.EmployeeNumber} already exists.");
            }

            // Checks whether the email already exists.
            var emailExists = await dbContext.Teachers
                .AnyAsync(t => t.Email == dto.Email, cancellationToken);

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
                    $"Department with ID {dto.DepartmentId} does not exist.");
            }

            var teacher = new TeacherEntity
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                EmployeeNumber = dto.EmployeeNumber,
                PhoneNumber = dto.PhoneNumber,
                DepartmentId = dto.DepartmentId
            };

            await dbContext.Teachers.AddAsync(teacher, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new TeacherDto
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                EmployeeNumber = teacher.EmployeeNumber,
                PhoneNumber = teacher.PhoneNumber,
                DepartmentId = teacher.DepartmentId
            };
        }
    }
}
