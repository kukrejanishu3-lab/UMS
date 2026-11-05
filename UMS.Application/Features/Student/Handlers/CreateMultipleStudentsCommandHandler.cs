using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.Features.Student.Commands;
using UMS.Application.Interfaces;
using StudentEntity = UMS.Domain.Entities.Student;

namespace UMS.Application.Features.Student.Handlers
{
    // Handles registration of multiple students.
    public class CreateMultipleStudentsCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<
            CreateMultipleStudentsCommand,
            (List<StudentEntity> AddedStudents, List<object> RejectedStudents)>
    {
        public async Task<(
            List<StudentEntity> AddedStudents,
            List<object> RejectedStudents)> Handle(
                CreateMultipleStudentsCommand request,
                CancellationToken cancellationToken)
        {
            var addedStudents = new List<StudentEntity>();
            var rejectedStudents = new List<object>();

            var studentNumbersInRequest = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var emailsInRequest = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var requestNumbers = request.Students.Select(s => s.StudentNumber).Distinct().ToList();
            var existingNumbers = (await dbContext.Students
                .Where(s => requestNumbers.Contains(s.StudentNumber))
                .Select(s => s.StudentNumber)
                .ToListAsync(cancellationToken))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var requestEmails = request.Students.Select(s => s.Email).Distinct().ToList();
            var existingEmails = (await dbContext.Students
                .Where(s => requestEmails.Contains(s.Email))
                .Select(s => s.Email)
                .ToListAsync(cancellationToken))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var validDepartmentIds = (await dbContext.Departments
                .Select(d => d.Id)
                .ToListAsync(cancellationToken))
                .ToHashSet();

            foreach (var dto in request.Students)
            {
                // Checks for duplicate student numbers in this request.
                if (!studentNumbersInRequest.Add(dto.StudentNumber))
                {
                    rejectedStudents.Add(new
                    {
                        studentNumber = dto.StudentNumber,
                        message = $"Student number {dto.StudentNumber} is duplicated in this request."
                    });
                    continue;
                }

                // Checks whether the student number already exists in the database.
                if (existingNumbers.Contains(dto.StudentNumber))
                {
                    rejectedStudents.Add(new
                    {
                        studentNumber = dto.StudentNumber,
                        message = $"Student number {dto.StudentNumber} already exists."
                    });
                    continue;
                }

                // Checks for duplicate emails in this request.
                if (!emailsInRequest.Add(dto.Email))
                {
                    rejectedStudents.Add(new
                    {
                        studentNumber = dto.StudentNumber,
                        message = $"Email {dto.Email} is duplicated in this request."
                    });
                    continue;
                }

                // Checks whether the email already exists in the database.
                if (existingEmails.Contains(dto.Email))
                {
                    rejectedStudents.Add(new
                    {
                        studentNumber = dto.StudentNumber,
                        message = $"Email {dto.Email} is already registered."
                    });
                    continue;
                }

                // Checks whether the department exists.
                if (!validDepartmentIds.Contains(dto.DepartmentId))
                {
                    rejectedStudents.Add(new
                    {
                        studentNumber = dto.StudentNumber,
                        message = $"Department {dto.DepartmentId} does not exist."
                    });
                    continue;
                }

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

                existingNumbers.Add(dto.StudentNumber);
                existingEmails.Add(dto.Email);

                addedStudents.Add(student);
            }

            return (addedStudents, rejectedStudents);
        }
    }
}
