using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Teacher;
using UMS.Application.Features.Teacher.Commands;
using UMS.Application.Interfaces;
using TeacherEntity = UMS.Domain.Entities.Teacher;

namespace UMS.Application.Features.Teacher.Handlers
{
    // Handles bulk registration of teachers.
    public class CreateMultipleTeachersCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<
            CreateMultipleTeachersCommand,
            (List<TeacherDto> AddedTeachers, List<object> RejectedTeachers)>
    {
        public async Task<(
            List<TeacherDto> AddedTeachers,
            List<object> RejectedTeachers)> Handle(
                CreateMultipleTeachersCommand request,
                CancellationToken cancellationToken)
        {
            var addedTeachers = new List<TeacherDto>();
            var rejectedTeachers = new List<object>();

            var employeeNumbersInRequest = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var emailsInRequest = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var requestNumbers = request.Teachers.Select(t => t.EmployeeNumber).Distinct().ToList();
            var existingNumbers = (await dbContext.Teachers
                .Where(t => requestNumbers.Contains(t.EmployeeNumber))
                .Select(t => t.EmployeeNumber)
                .ToListAsync(cancellationToken))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var requestEmails = request.Teachers.Select(t => t.Email).Distinct().ToList();
            var existingEmails = (await dbContext.Teachers
                .Where(t => requestEmails.Contains(t.Email))
                .Select(t => t.Email)
                .ToListAsync(cancellationToken))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var validDepartmentIds = (await dbContext.Departments
                .Select(d => d.Id)
                .ToListAsync(cancellationToken))
                .ToHashSet();

            foreach (var dto in request.Teachers)
            {
                // Checks for duplicate employee numbers within this request.
                if (!employeeNumbersInRequest.Add(dto.EmployeeNumber))
                {
                    rejectedTeachers.Add(new
                    {
                        employeeNumber = dto.EmployeeNumber,
                        message = $"Employee number {dto.EmployeeNumber} is duplicated in this request."
                    });
                    continue;
                }

                // Checks whether the employee number already exists in the database.
                if (existingNumbers.Contains(dto.EmployeeNumber))
                {
                    rejectedTeachers.Add(new
                    {
                        employeeNumber = dto.EmployeeNumber,
                        message = $"Employee number {dto.EmployeeNumber} already exists."
                    });
                    continue;
                }

                // Checks for duplicate emails within this request.
                if (!emailsInRequest.Add(dto.Email))
                {
                    rejectedTeachers.Add(new
                    {
                        employeeNumber = dto.EmployeeNumber,
                        message = $"Email {dto.Email} is duplicated in this request."
                    });
                    continue;
                }

                // Checks whether the email already exists in the database.
                if (existingEmails.Contains(dto.Email))
                {
                    rejectedTeachers.Add(new
                    {
                        employeeNumber = dto.EmployeeNumber,
                        message = $"Email {dto.Email} already exists."
                    });
                    continue;
                }

                // Checks whether the department exists.
                if (!validDepartmentIds.Contains(dto.DepartmentId))
                {
                    rejectedTeachers.Add(new
                    {
                        employeeNumber = dto.EmployeeNumber,
                        message = $"Department with ID {dto.DepartmentId} does not exist."
                    });
                    continue;
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

                existingNumbers.Add(dto.EmployeeNumber);
                existingEmails.Add(dto.Email);

                addedTeachers.Add(new TeacherDto
                {
                    Id = teacher.Id,
                    FirstName = teacher.FirstName,
                    LastName = teacher.LastName,
                    Email = teacher.Email,
                    EmployeeNumber = teacher.EmployeeNumber,
                    PhoneNumber = teacher.PhoneNumber,
                    DepartmentId = teacher.DepartmentId
                });
            }

            return (addedTeachers, rejectedTeachers);
        }
    }
}
