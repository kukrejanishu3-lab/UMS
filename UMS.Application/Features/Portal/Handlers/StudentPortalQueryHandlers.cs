using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Portal;
using UMS.Application.Features.Portal.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Portal.Handlers
{
    public class GetStudentMyCoursesQueryHandler : IRequestHandler<GetStudentMyCoursesQuery, List<StudentCourseDto>>
    {
        private readonly IAppDbContext _context;

        public GetStudentMyCoursesQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentCourseDto>> Handle(GetStudentMyCoursesQuery request, CancellationToken cancellationToken)
        {
            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == request.UserEmail, cancellationToken);

            if (student is null)
            {
                throw new KeyNotFoundException($"Student profile not found for '{request.UserEmail}'.");
            }

            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == student.Id && (e.Status == "Enrolled" || e.Status == "Dropped"))
                .Include(e => e.Section)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Section)
                    .ThenInclude(s => s.Semester)
                .ToListAsync(cancellationToken);

            return enrollments.Select(e => new StudentCourseDto
            {
                CourseId = e.Section.CourseId,
                CourseName = e.Section.Course.Name,
                CourseCode = e.Section.Course.Code,
                CreditHours = e.Section.Course.CreditHours,
                SectionId = e.SectionId,
                SectionName = e.Section.Name,
                SemesterName = e.Section.Semester.Name,
                EnrollmentStatus = e.Status,
                Grade = e.Grade,
                GradePoints = e.GradePoints
            }).ToList();
        }
    }

    public class GetStudentMyScheduleQueryHandler : IRequestHandler<GetStudentMyScheduleQuery, List<StudentScheduleDto>>
    {
        private readonly IAppDbContext _context;

        public GetStudentMyScheduleQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentScheduleDto>> Handle(GetStudentMyScheduleQuery request, CancellationToken cancellationToken)
        {
            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == request.UserEmail, cancellationToken);

            if (student is null)
            {
                throw new KeyNotFoundException($"Student profile not found for '{request.UserEmail}'.");
            }

            var activeSectionIds = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == student.Id && e.Status == "Enrolled")
                .Select(e => e.SectionId)
                .ToListAsync(cancellationToken);

            var timetables = await _context.Timetables
                .AsNoTracking()
                .Where(t => activeSectionIds.Contains(t.SectionId))
                .Include(t => t.Section)
                    .ThenInclude(s => s.Course)
                .Include(t => t.Teacher)
                .ToListAsync(cancellationToken);

            return timetables.Select(t =>
            {
                var duration = t.EndTime - t.StartTime;
                var durationStr = duration.TotalMinutes >= 60
                    ? $"{(int)duration.TotalHours} hr {(duration.Minutes > 0 ? duration.Minutes + " mins" : "")}".Trim()
                    : $"{(int)duration.TotalMinutes} mins";

                return new StudentScheduleDto
                {
                    TimetableId = t.Id,
                    Day = t.DayOfWeek.ToString(),
                    CourseName = t.Section.Course.Name,
                    CourseCode = t.Section.Course.Code,
                    SectionName = t.Section.Name,
                    TeacherName = t.Teacher != null ? $"{t.Teacher.FirstName} {t.Teacher.LastName}".Trim() : "TBD",
                    StartTime = t.StartTime.ToString("hh:mm tt"),
                    EndTime = t.EndTime.ToString("hh:mm tt"),
                    Duration = durationStr,
                    RoomNumber = t.RoomNumber
                };
            }).ToList();
        }
    }

    public class GetStudentMyDropRequestsQueryHandler : IRequestHandler<GetStudentMyDropRequestsQuery, List<StudentDropRequestDto>>
    {
        private readonly IAppDbContext _context;

        public GetStudentMyDropRequestsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentDropRequestDto>> Handle(GetStudentMyDropRequestsQuery request, CancellationToken cancellationToken)
        {
            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == request.UserEmail, cancellationToken);

            if (student is null)
            {
                throw new KeyNotFoundException($"Student profile not found for '{request.UserEmail}'.");
            }

            var dropRequests = await _context.DropCourseRequests
                .AsNoTracking()
                .Where(r => r.Enrollment.StudentId == student.Id)
                .Include(r => r.Enrollment)
                    .ThenInclude(e => e.Section)
                        .ThenInclude(s => s.Course)
                .Include(r => r.Enrollment)
                    .ThenInclude(e => e.Section)
                        .ThenInclude(s => s.Semester)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync(cancellationToken);

            return dropRequests.Select(r => new StudentDropRequestDto
            {
                RequestId = r.Id,
                EnrollmentId = r.EnrollmentId,
                CourseName = r.Enrollment.Section?.Course?.Name ?? string.Empty,
                CourseCode = r.Enrollment.Section?.Course?.Code ?? string.Empty,
                SectionName = r.Enrollment.Section?.Name ?? string.Empty,
                SemesterName = r.Enrollment.Section?.Semester?.Name ?? string.Empty,
                Reason = r.Reason,
                Status = r.Status,
                RequestDate = r.RequestDate,
                AdminRemarks = r.AdminRemarks,
                Grade = r.Enrollment.Grade
            }).ToList();
        }
    }
}


