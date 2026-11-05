using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Portal;
using UMS.Application.Features.Portal.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Portal.Handlers
{
    public class GetTeacherMySectionsQueryHandler : IRequestHandler<GetTeacherMySectionsQuery, List<TeacherSectionDto>>
    {
        private readonly IAppDbContext _context;

        public GetTeacherMySectionsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TeacherSectionDto>> Handle(GetTeacherMySectionsQuery request, CancellationToken cancellationToken)
        {
            var teacher = await _context.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == request.UserEmail, cancellationToken);

            if (teacher is null)
            {
                throw new KeyNotFoundException($"Teacher profile not found for '{request.UserEmail}'.");
            }

            var assignedSectionIds = await _context.SectionTeachers
                .AsNoTracking()
                .Where(st => st.TeacherId == teacher.Id)
                .Select(st => st.SectionId)
                .Union(_context.Timetables
                    .AsNoTracking()
                    .Where(t => t.TeacherId == teacher.Id)
                    .Select(t => t.SectionId))
                .Distinct()
                .ToListAsync(cancellationToken);

            var sections = await _context.Sections
                .AsNoTracking()
                .Where(s => assignedSectionIds.Contains(s.Id))
                .Include(s => s.Course)
                .Include(s => s.Semester)
                .ToListAsync(cancellationToken);

            var activeEnrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => assignedSectionIds.Contains(e.SectionId) && e.Status == "Enrolled")
                .Select(e => e.SectionId)
                .ToListAsync(cancellationToken);

            var countsBySection = activeEnrollments
                .GroupBy(sId => sId)
                .ToDictionary(g => g.Key, g => g.Count());

            return sections.Select(s => new TeacherSectionDto
            {
                SectionId = s.Id,
                SectionName = s.Name,
                CourseId = s.CourseId,
                CourseName = s.Course.Name,
                CourseCode = s.Course.Code,
                SemesterName = s.Semester.Name,
                Capacity = s.Capacity,
                EnrolledStudentsCount = countsBySection.TryGetValue(s.Id, out var count) ? count : 0
            }).ToList();
        }
    }

    public class GetTeacherMyScheduleQueryHandler : IRequestHandler<GetTeacherMyScheduleQuery, List<TeacherScheduleDto>>
    {
        private readonly IAppDbContext _context;

        public GetTeacherMyScheduleQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TeacherScheduleDto>> Handle(GetTeacherMyScheduleQuery request, CancellationToken cancellationToken)
        {
            var teacher = await _context.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == request.UserEmail, cancellationToken);

            if (teacher is null)
            {
                throw new KeyNotFoundException($"Teacher profile not found for '{request.UserEmail}'.");
            }

            var timetables = await _context.Timetables
                .AsNoTracking()
                .Where(t => t.TeacherId == teacher.Id)
                .Include(t => t.Section)
                    .ThenInclude(s => s.Course)
                .ToListAsync(cancellationToken);

            return timetables.Select(t =>
            {
                var duration = t.EndTime - t.StartTime;
                var durationStr = duration.TotalMinutes >= 60
                    ? $"{(int)duration.TotalHours} hr {(duration.Minutes > 0 ? duration.Minutes + " mins" : "")}".Trim()
                    : $"{(int)duration.TotalMinutes} mins";

                return new TeacherScheduleDto
                {
                    TimetableId = t.Id,
                    Day = t.DayOfWeek.ToString(),
                    CourseName = t.Section.Course.Name,
                    CourseCode = t.Section.Course.Code,
                    SectionName = t.Section.Name,
                    StartTime = t.StartTime.ToString("hh:mm tt"),
                    EndTime = t.EndTime.ToString("hh:mm tt"),
                    Duration = durationStr,
                    RoomNumber = t.RoomNumber
                };
            }).ToList();
        }
    }

    public class GetTeacherSectionStudentsQueryHandler : IRequestHandler<GetTeacherSectionStudentsQuery, List<SectionStudentRosterDto>>
    {
        private readonly IAppDbContext _context;

        public GetTeacherSectionStudentsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SectionStudentRosterDto>> Handle(GetTeacherSectionStudentsQuery request, CancellationToken cancellationToken)
        {
            var teacher = await _context.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == request.UserEmail, cancellationToken);

            if (teacher is null)
            {
                throw new KeyNotFoundException($"Teacher profile not found for '{request.UserEmail}'.");
            }

            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.SectionId == request.SectionId && e.Status == "Enrolled")
                .Include(e => e.Student)
                .ToListAsync(cancellationToken);

            return enrollments.Select(e => new SectionStudentRosterDto
            {
                StudentId = e.StudentId,
                StudentName = e.Student != null ? $"{e.Student.FirstName} {e.Student.LastName}".Trim() : string.Empty,
                StudentNumber = e.Student != null ? e.Student.StudentNumber : string.Empty,
                Email = e.Student != null ? e.Student.Email : string.Empty,
                PhoneNumber = e.Student != null ? e.Student.PhoneNumber : string.Empty
            }).ToList();
        }
    }
}
