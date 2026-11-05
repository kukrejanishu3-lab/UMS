using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Teacher;
using UMS.Application.Features.Teacher.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Teacher.Handlers
{
    // Handles retrieving a teacher by ID with complete academic department, assigned sections, and teaching routine.
    public class GetTeacherByIdQueryHandler(IAppDbContext dbContext)
        : IRequestHandler<GetTeacherByIdQuery, TeacherDto?>
    {
        public async Task<TeacherDto?> Handle(
            GetTeacherByIdQuery request,
            CancellationToken cancellationToken)
        {
            var t = await dbContext.Teachers
                .AsNoTracking()
                .Include(teacher => teacher.Department)
                .FirstOrDefaultAsync(teacher => teacher.Id == request.TeacherId, cancellationToken);

            if (t is null) return null;

            var sectionTeachers = await dbContext.SectionTeachers
                .AsNoTracking()
                .Where(st => st.TeacherId == t.Id)
                .Include(st => st.Section)
                    .ThenInclude(s => s.Course)
                .ToListAsync(cancellationToken);

            var timetables = await dbContext.Timetables
                .AsNoTracking()
                .Where(tt => tt.TeacherId == t.Id)
                .Include(tt => tt.Section)
                    .ThenInclude(s => s.Course)
                .OrderBy(tt => tt.DayOfWeek)
                .ThenBy(tt => tt.StartTime)
                .ToListAsync(cancellationToken);

            var distinctSections = sectionTeachers
                .Where(st => st.Section != null)
                .Select(st => new TeacherAssignedSectionDto
                {
                    SectionId = st.SectionId,
                    SectionName = st.Section.Name,
                    CourseName = st.Section.Course?.Name ?? string.Empty,
                    CourseCode = st.Section.Course?.Code ?? string.Empty
                })
                .Concat(timetables
                    .Where(tt => tt.Section != null)
                    .Select(tt => new TeacherAssignedSectionDto
                    {
                        SectionId = tt.SectionId,
                        SectionName = tt.Section.Name,
                        CourseName = tt.Section.Course?.Name ?? string.Empty,
                        CourseCode = tt.Section.Course?.Code ?? string.Empty
                    }))
                .GroupBy(s => s.SectionId)
                .Select(g => g.First())
                .ToList();

            var scheduleSlots = timetables.Select(tt =>
            {
                var duration = tt.EndTime - tt.StartTime;
                var durationStr = duration.TotalMinutes >= 60
                    ? $"{(int)duration.TotalHours} hr {(duration.Minutes > 0 ? duration.Minutes + " mins" : "")}".Trim()
                    : $"{(int)duration.TotalMinutes} mins";

                return new TeacherScheduleSlotDto
                {
                    TimetableId = tt.Id,
                    Day = tt.DayOfWeek.ToString(),
                    StartTime = tt.StartTime.ToString("hh:mm tt"),
                    EndTime = tt.EndTime.ToString("hh:mm tt"),
                    Duration = durationStr,
                    RoomNumber = tt.RoomNumber,
                    SectionName = tt.Section?.Name ?? string.Empty,
                    CourseName = tt.Section?.Course?.Name ?? string.Empty
                };
            }).ToList();

            return new TeacherDto
            {
                Id = t.Id,
                FirstName = t.FirstName,
                LastName = t.LastName,
                Email = t.Email,
                EmployeeNumber = t.EmployeeNumber,
                PhoneNumber = t.PhoneNumber,
                DepartmentId = t.DepartmentId,
                DepartmentName = t.Department != null ? t.Department.Name : string.Empty,
                TotalAssignedSections = distinctSections.Count,
                AssignedSections = distinctSections,
                TeachingSchedule = scheduleSlots
            };
        }
    }
}

