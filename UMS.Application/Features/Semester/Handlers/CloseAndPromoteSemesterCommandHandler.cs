using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Semester;
using UMS.Application.Features.Semester.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Semester.Handlers
{
    public class CloseAndPromoteSemesterCommandHandler : IRequestHandler<CloseAndPromoteSemesterCommand, CloseSemesterResultDto>
    {
        private readonly IAppDbContext _context;

        public CloseAndPromoteSemesterCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<CloseSemesterResultDto> Handle(CloseAndPromoteSemesterCommand request, CancellationToken cancellationToken)
        {
            // 1. Verify Semester exists
            var semester = await _context.Semesters
                .FirstOrDefaultAsync(s => s.Id == request.SemesterId, cancellationToken);

            if (semester is null)
            {
                throw new KeyNotFoundException("Semester not found.");
            }

            if (semester.IsClosed)
            {
                throw new InvalidOperationException($"Semester '{semester.Name}' is already closed and finalized.");
            }

            // 2. Lock / Close Semester
            semester.IsClosed = true;
            semester.IsActive = false;

            // 3. Fetch all enrollments in this closing semester
            var closingEnrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Section)
                    .ThenInclude(s => s.Course)
                .Where(e => e.Section.SemesterId == request.SemesterId)
                .ToListAsync(cancellationToken);

            var studentIds = closingEnrollments
                .Select(e => e.StudentId)
                .Distinct()
                .ToList();

            var promotedStudents = new List<PromotedStudentDto>();
            var probationStudents = new List<PromotedStudentDto>();
            var repeatCourses = new List<RepeaterCourseDto>();

            // Fetch all student entities for update
            var students = await _context.Students
                .Where(s => studentIds.Contains(s.Id))
                .ToListAsync(cancellationToken);

            // Fetch all historical enrollments of these students to calculate cumulative CGPA
            var allHistoricalEnrollments = await _context.Enrollments
                .Include(e => e.Section)
                    .ThenInclude(s => s.Course)
                .Where(e => studentIds.Contains(e.StudentId))
                .ToListAsync(cancellationToken);

            foreach (var student in students)
            {
                var studentCurrentSemEnrollments = closingEnrollments
                    .Where(e => e.StudentId == student.Id)
                    .ToList();

                var studentAllEnrollments = allHistoricalEnrollments
                    .Where(e => e.StudentId == student.Id)
                    .ToList();

                // Compute Semester SGPA (for closing semester)
                int semCreditsAttempted = 0;
                decimal semQualityPoints = 0;

                foreach (var e in studentCurrentSemEnrollments)
                {
                    int ch = e.Section?.Course?.CreditHours ?? 0;
                    bool isDropped = string.Equals(e.Grade, "W", StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(e.Status, "Dropped", StringComparison.OrdinalIgnoreCase);

                    if (isDropped)
                    {
                        repeatCourses.Add(new RepeaterCourseDto
                        {
                            StudentId = student.Id,
                            StudentRollNo = student.StudentNumber,
                            StudentName = $"{student.FirstName} {student.LastName}".Trim(),
                            CourseCode = e.Section?.Course?.Code ?? "N/A",
                            CourseName = e.Section?.Course?.Name ?? "N/A",
                            CreditHours = ch,
                            Grade = "W",
                            RequirementType = "Dropped (W) - Course Registration Required"
                        });
                        continue;
                    }

                    bool isFail = string.Equals(e.Grade, "F", StringComparison.OrdinalIgnoreCase);
                    semCreditsAttempted += ch;

                    if (isFail)
                    {
                        repeatCourses.Add(new RepeaterCourseDto
                        {
                            StudentId = student.Id,
                            StudentRollNo = student.StudentNumber,
                            StudentName = $"{student.FirstName} {student.LastName}".Trim(),
                            CourseCode = e.Section?.Course?.Code ?? "N/A",
                            CourseName = e.Section?.Course?.Name ?? "N/A",
                            CreditHours = ch,
                            Grade = "F",
                            RequirementType = "Failed (F) - Mandatory Course Repeat"
                        });
                    }
                    else
                    {
                        semQualityPoints += ch * (e.GradePoints ?? 0m);
                    }
                }

                decimal sgpa = semCreditsAttempted > 0
                    ? Math.Round(semQualityPoints / semCreditsAttempted, 2)
                    : 0.00m;

                // Compute Cumulative CGPA across all completed semesters
                int cumCreditsAttempted = 0;
                decimal cumQualityPoints = 0;

                foreach (var e in studentAllEnrollments)
                {
                    int ch = e.Section?.Course?.CreditHours ?? 0;
                    bool isDropped = string.Equals(e.Grade, "W", StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(e.Status, "Dropped", StringComparison.OrdinalIgnoreCase);

                    if (!isDropped && !string.IsNullOrWhiteSpace(e.Grade))
                    {
                        cumCreditsAttempted += ch;
                        cumQualityPoints += ch * (e.GradePoints ?? 0m);
                    }
                }

                decimal cgpa = cumCreditsAttempted > 0
                    ? Math.Round(cumQualityPoints / cumCreditsAttempted, 2)
                    : sgpa;

                int previousSemester = student.CurrentSemester;
                int newSemester = previousSemester + 1; // Promote to next academic semester

                student.CurrentSemester = newSemester;
                student.CGPA = cgpa;

                bool isOnProbation = cgpa < 2.00m;
                string standing = isOnProbation
                    ? "Academic Probation Warning"
                    : (cgpa >= 3.50m) ? "Dean's Honor List / Excellent" : "Good Standing";

                student.AcademicStanding = standing;

                var studentSummary = new PromotedStudentDto
                {
                    StudentId = student.Id,
                    StudentRollNo = student.StudentNumber,
                    StudentName = $"{student.FirstName} {student.LastName}".Trim(),
                    PreviousSemester = previousSemester,
                    NewSemester = newSemester,
                    SemesterSGPA = sgpa,
                    CumulativeCGPA = cgpa,
                    AcademicStanding = standing,
                    PromotionStatus = isOnProbation
                        ? $"Promoted to Semester {newSemester} on Academic Probation"
                        : $"Promoted to Semester {newSemester} in {standing}"
                };

                if (isOnProbation)
                {
                    probationStudents.Add(studentSummary);
                }
                else
                {
                    promotedStudents.Add(studentSummary);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new CloseSemesterResultDto
            {
                SemesterId = semester.Id,
                SemesterName = semester.Name,
                ClosedDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TotalStudentsEvaluated = students.Count,
                TotalPromoted = promotedStudents.Count,
                TotalOnProbation = probationStudents.Count,
                TotalRepeatersFlagged = repeatCourses.Count,
                Message = $"Semester '{semester.Name}' closed successfully. Results locked and students evaluated for promotion.",
                PromotedStudents = promotedStudents,
                ProbationStudents = probationStudents,
                RepeatRequiredCourses = repeatCourses
            };
        }
    }
}

