using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Grading;
using UMS.Application.Features.Grading.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Grading.Handlers
{
    public class SubmitCourseGradesCommandHandler : IRequestHandler<SubmitCourseGradesCommand, SubmitCourseGradesResultDto>
    {
        private readonly IAppDbContext _context;

        public SubmitCourseGradesCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<SubmitCourseGradesResultDto> Handle(SubmitCourseGradesCommand request, CancellationToken cancellationToken)
        {
            // 1. Verify caller identity (Teacher or Admin)
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => (request.UserId.HasValue && u.Id == request.UserId) || u.Email == request.UserEmail, cancellationToken);

            bool isAdmin = user != null && (user.Role == "Admin" || user.Role == "SuperAdmin");

            var teacher = await _context.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == request.UserEmail, cancellationToken);

            if (!isAdmin && teacher is null)
            {
                throw new KeyNotFoundException($"Teacher profile not found for '{request.UserEmail}'.");
            }

            // 2. Verify Section exists
            var section = await _context.Sections
                .Include(s => s.Course)
                .Include(s => s.Semester)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken);

            if (section is null)
            {
                throw new KeyNotFoundException("Section not found.");
            }

            // 3. Verify Teacher assignment (if not admin)
            if (!isAdmin && teacher is not null)
            {
                var isAssigned = await _context.SectionTeachers
                    .AnyAsync(st => st.SectionId == request.SectionId && st.TeacherId == teacher.Id, cancellationToken)
                    || await _context.Timetables
                    .AnyAsync(t => t.SectionId == request.SectionId && t.TeacherId == teacher.Id, cancellationToken);

                if (!isAssigned)
                {
                    throw new InvalidOperationException($"Teacher conflict: You are not assigned to teach section '{section.Name}' and cannot submit grades for it.");
                }
            }

            // 4. Fetch enrollments in this section
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.SectionId == request.SectionId)
                .ToListAsync(cancellationToken);

            var resultRecords = new List<StudentGradeRecordDto>();

            foreach (var item in request.Grades)
            {
                var enrollment = enrollments.FirstOrDefault(e => e.StudentId == item.StudentId);
                if (enrollment is null)
                {
                    continue; // Skip or ignore students not enrolled in this section
                }

                // Protect dropped course enrollments: Grade 'W' is locked
                if (string.Equals(enrollment.Status, "Dropped", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(enrollment.Grade, "W", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Cannot assign marks to student '{enrollment.Student.StudentNumber}': The course was dropped and grade is locked as 'W'.");
                }

                if (item.Marks < 0 || item.Marks > 100)
                {
                    throw new ArgumentException($"Invalid marks '{item.Marks}' for student '{enrollment.Student.StudentNumber}'. Marks must be between 0 and 100.");
                }

                var (letterGrade, gradePoints) = CalculateHecGrade(item.Marks);

                enrollment.Marks = item.Marks;
                enrollment.Grade = letterGrade;
                enrollment.GradePoints = gradePoints;

                resultRecords.Add(new StudentGradeRecordDto
                {
                    EnrollmentId = enrollment.Id,
                    StudentId = enrollment.StudentId,
                    StudentRollNo = enrollment.Student.StudentNumber,
                    StudentName = $"{enrollment.Student.FirstName} {enrollment.Student.LastName}".Trim(),
                    Marks = item.Marks,
                    Grade = letterGrade,
                    GradePoints = gradePoints,
                    Remarks = item.Remarks
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new SubmitCourseGradesResultDto
            {
                SectionId = section.Id,
                SectionName = section.Name,
                CourseCode = section.Course?.Code ?? "N/A",
                CourseName = section.Course?.Name ?? "N/A",
                TotalGraded = resultRecords.Count,
                Message = $"Grades submitted successfully for '{section.Course?.Name}' ({section.Name}).",
                Grades = resultRecords.OrderBy(r => r.StudentRollNo).ToList()
            };
        }

        // Standard HEC Grading Conversion
        public static (string Grade, decimal GradePoints) CalculateHecGrade(decimal marks)
        {
            if (marks >= 85.0m) return ("A", 4.00m);
            if (marks >= 80.0m) return ("A-", 3.67m);
            if (marks >= 75.0m) return ("B+", 3.33m);
            if (marks >= 70.0m) return ("B", 3.00m);
            if (marks >= 65.0m) return ("B-", 2.67m);
            if (marks >= 61.0m) return ("C+", 2.33m);
            if (marks >= 58.0m) return ("C", 2.00m);
            if (marks >= 55.0m) return ("C-", 1.67m);
            if (marks >= 50.0m) return ("D", 1.00m);
            return ("F", 0.00m);
        }
    }
}
