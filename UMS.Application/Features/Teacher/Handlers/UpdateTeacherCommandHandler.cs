using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Teacher;
using UMS.Application.Features.Teacher.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Teacher.Handlers
{
    // Handles updating an existing teacher.
    public class UpdateTeacherCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<UpdateTeacherCommand, TeacherDto?>
    {
        public async Task<TeacherDto?> Handle(
            UpdateTeacherCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.TeacherDto;

            var existingTeacher = await dbContext.Teachers
                .FirstOrDefaultAsync(t => t.Id == request.TeacherId, cancellationToken);

            if (existingTeacher is null)
            {
                return null;
            }

            // Checks whether the employee number is taken by another teacher.
            var employeeNumberExists = await dbContext.Teachers
                .AnyAsync(t => t.EmployeeNumber == dto.EmployeeNumber && t.Id != request.TeacherId, cancellationToken);

            if (employeeNumberExists)
            {
                throw new InvalidOperationException(
                    $"Employee number {dto.EmployeeNumber} already exists.");
            }

            // Checks whether the email is taken by another teacher.
            var emailExists = await dbContext.Teachers
                .AnyAsync(t => t.Email == dto.Email && t.Id != request.TeacherId, cancellationToken);

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

            existingTeacher.FirstName = dto.FirstName;
            existingTeacher.LastName = dto.LastName;
            existingTeacher.Email = dto.Email;
            existingTeacher.EmployeeNumber = dto.EmployeeNumber;
            existingTeacher.PhoneNumber = dto.PhoneNumber;
            existingTeacher.DepartmentId = dto.DepartmentId;

            await dbContext.SaveChangesAsync(cancellationToken);

            return new TeacherDto
            {
                Id = existingTeacher.Id,
                FirstName = existingTeacher.FirstName,
                LastName = existingTeacher.LastName,
                Email = existingTeacher.Email,
                EmployeeNumber = existingTeacher.EmployeeNumber,
                PhoneNumber = existingTeacher.PhoneNumber,
                DepartmentId = existingTeacher.DepartmentId
            };
        }
    }
}
