using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Grading;
using UMS.Application.Features.Grading.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Grading.Handlers
{
    public class GetSectionGradesQueryHandler : IRequestHandler<GetSectionGradesQuery, SectionGradeSheetDto>
    {
        private readonly IAppDbContext _context;

        public GetSectionGradesQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<SectionGradeSheetDto> Handle(GetSectionGradesQuery request, CancellationToken cancellationToken)
        {
            var section = await _context.Sections
                .Include(s => s.Course)
                .Include(s => s.Semester)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken);

            if (section is null)
            {
                throw new KeyNotFoundException("Section not found.");
            }

            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .AsNoTracking()
                .Where(e => e.SectionId == request.SectionId)
                .OrderBy(e => e.Student.StudentNumber)
                .ToListAsync(cancellationToken);

            var gradeRecords = enrollments.Select(e => new StudentGradeRecordDto
            {
                EnrollmentId = e.Id,
                StudentId = e.StudentId,
                StudentRollNo = e.Student.StudentNumber,
                StudentName = $"{e.Student.FirstName} {e.Student.LastName}".Trim(),
                Marks = e.Marks,
                Grade = e.Grade,
                GradePoints = e.GradePoints,
                Remarks = string.Equals(e.Status, "Dropped", StringComparison.OrdinalIgnoreCase) ? "Course Dropped (W)" : null
            }).ToList();

            return new SectionGradeSheetDto
            {
                SectionId = section.Id,
                SectionName = section.Name,
                CourseCode = section.Course?.Code ?? "N/A",
                CourseName = section.Course?.Name ?? "N/A",
                SemesterName = section.Semester?.Name ?? "N/A",
                CreditHours = section.Course?.CreditHours ?? 0,
                Grades = gradeRecords
            };
        }
    }
}
