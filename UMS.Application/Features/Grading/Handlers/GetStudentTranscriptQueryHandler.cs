using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Grading;
using UMS.Application.Features.Grading.Queries;
using UMS.Application.Interfaces;
using UMS.Domain.Entities;

namespace UMS.Application.Features.Grading.Handlers
{
    public class GetStudentTranscriptQueryHandler : IRequestHandler<GetStudentTranscriptQuery, StudentTranscriptDto>
    {
        private readonly IAppDbContext _context;

        public GetStudentTranscriptQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<StudentTranscriptDto> Handle(GetStudentTranscriptQuery request, CancellationToken cancellationToken)
        {
            UMS.Domain.Entities.Student? student = null;

            if (request.StudentId.HasValue && request.StudentId.Value != Guid.Empty)
            {
                student = await _context.Students
                    .Include(s => s.Department)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == request.StudentId.Value, cancellationToken);

                if (student is null)
                {
                    throw new KeyNotFoundException($"Student profile not found for ID '{request.StudentId.Value}'.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.UserEmail))
            {
                student = await _context.Students
                    .Include(s => s.Department)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Email == request.UserEmail, cancellationToken);

                if (student is null)
                {
                    throw new KeyNotFoundException($"Student profile not found for '{request.UserEmail}'.");
                }
            }
            else
            {
                throw new ArgumentException("Student identifier (ID or Email) must be provided.");
            }

            var enrollments = await _context.Enrollments
                .Include(e => e.Section)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Section)
                    .ThenInclude(s => s.Semester)
                .AsNoTracking()
                .Where(e => e.StudentId == student.Id)
                .ToListAsync(cancellationToken);

            // Group enrollments by Semester
            var semesterGroups = enrollments
                .GroupBy(e => new
                {
                    SemesterId = e.Section?.SemesterId ?? Guid.Empty,
                    SemesterName = e.Section?.Semester?.Name ?? "Unassigned",
                    StartDate = e.Section?.Semester?.StartDate ?? DateOnly.MinValue
                })
                .OrderBy(g => g.Key.StartDate)
                .ToList();

            var semesterTranscripts = new List<SemesterTranscriptDto>();

            int cumulativeCreditsAttempted = 0;
            int cumulativeCreditsEarned = 0;
            decimal cumulativeQualityPoints = 0;

            foreach (var group in semesterGroups)
            {
                var courses = new List<StudentTranscriptCourseDto>();
                int semCreditsAttempted = 0;
                int semCreditsEarned = 0;
                decimal semQualityPoints = 0;

                foreach (var e in group)
                {
                    int ch = e.Section?.Course?.CreditHours ?? 0;
                    bool isDropped = string.Equals(e.Grade, "W", StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(e.Status, "Dropped", StringComparison.OrdinalIgnoreCase);

                    bool isFail = string.Equals(e.Grade, "F", StringComparison.OrdinalIgnoreCase);
                    bool isGraded = !string.IsNullOrWhiteSpace(e.Grade) && !isDropped;

                    decimal qp = 0;
                    string statusStr;

                    if (isDropped)
                    {
                        statusStr = "Dropped (W)";
                    }
                    else if (isFail)
                    {
                        statusStr = "Fail";
                        semCreditsAttempted += ch;
                    }
                    else if (isGraded)
                    {
                        statusStr = "Pass";
                        qp = ch * (e.GradePoints ?? 0m);
                        semCreditsAttempted += ch;
                        semCreditsEarned += ch;
                        semQualityPoints += qp;
                    }
                    else
                    {
                        statusStr = "In Progress";
                    }

                    courses.Add(new StudentTranscriptCourseDto
                    {
                        CourseCode = e.Section?.Course?.Code ?? "N/A",
                        CourseName = e.Section?.Course?.Name ?? "N/A",
                        CreditHours = ch,
                        Marks = e.Marks,
                        Grade = isDropped ? "W" : e.Grade,
                        GradePoints = isDropped ? null : e.GradePoints,
                        QualityPoints = qp,
                        Status = statusStr
                    });
                }

                decimal sgpa = semCreditsAttempted > 0
                    ? Math.Round(semQualityPoints / semCreditsAttempted, 2)
                    : 0.00m;

                cumulativeCreditsAttempted += semCreditsAttempted;
                cumulativeCreditsEarned += semCreditsEarned;
                cumulativeQualityPoints += semQualityPoints;

                decimal progressiveCgpa = cumulativeCreditsAttempted > 0
                    ? Math.Round(cumulativeQualityPoints / cumulativeCreditsAttempted, 2)
                    : sgpa;

                semesterTranscripts.Add(new SemesterTranscriptDto
                {
                    SemesterId = group.Key.SemesterId,
                    SemesterName = group.Key.SemesterName,
                    CreditsAttempted = semCreditsAttempted,
                    CreditsEarned = semCreditsEarned,
                    SemesterQualityPoints = semQualityPoints,
                    SGPA = sgpa,
                    CumulativeCGPA = progressiveCgpa,
                    Courses = courses
                });
            }

            decimal cgpa = cumulativeCreditsAttempted > 0
                ? Math.Round(cumulativeQualityPoints / cumulativeCreditsAttempted, 2)
                : 0.00m;

            string academicStanding;
            if (cumulativeCreditsAttempted == 0)
            {
                academicStanding = "New Student / Pending Evaluation";
            }
            else if (cgpa >= 3.50m)
            {
                academicStanding = "Dean's Honor List / Excellent Standing";
            }
            else if (cgpa >= 3.00m)
            {
                academicStanding = "Good Academic Standing";
            }
            else if (cgpa >= 2.00m)
            {
                academicStanding = "Satisfactory Academic Standing";
            }
            else
            {
                academicStanding = "Academic Probation Warning (CGPA below 2.00)";
            }

            return new StudentTranscriptDto
            {
                StudentId = student.Id,
                StudentRollNo = student.StudentNumber,
                StudentName = $"{student.FirstName} {student.LastName}".Trim(),
                DepartmentName = student.Department?.Name ?? "N/A",
                CurrentSemester = student.CurrentSemester,
                TotalCreditsAttempted = cumulativeCreditsAttempted,
                TotalCreditsEarned = cumulativeCreditsEarned,
                TotalQualityPoints = cumulativeQualityPoints,
                CGPA = cgpa,
                AcademicStanding = academicStanding,
                Semesters = semesterTranscripts
            };
        }
    }
}
