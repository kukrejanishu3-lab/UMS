using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Timetable;
using UMS.Application.Features.Timetable.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Timetable.Handlers
{
    // Handles retrieving a timetable entry by ID with rich details.
    public class GetTimetableByIdQueryHandler(IAppDbContext dbContext)
        : IRequestHandler<GetTimetableByIdQuery, TimetableDto?>
    {
        public async Task<TimetableDto?> Handle(
            GetTimetableByIdQuery request,
            CancellationToken cancellationToken)
        {
            var t = await dbContext.Timetables
                .AsNoTracking()
                .Include(t => t.Section)
                .Include(t => t.Teacher)
                .FirstOrDefaultAsync(x => x.Id == request.TimetableId, cancellationToken);

            if (t is null)
            {
                return null;
            }

            var duration = t.EndTime - t.StartTime;
            var durationStr = duration.TotalMinutes >= 60
                ? $"{(int)duration.TotalHours} hr {(duration.Minutes > 0 ? duration.Minutes + " mins" : "")}".Trim()
                : $"{(int)duration.TotalMinutes} mins";

            return new TimetableDto
            {
                Id = t.Id,
                SectionId = t.SectionId,
                SectionName = t.Section?.Name ?? string.Empty,
                TeacherId = t.TeacherId,
                TeacherName = t.Teacher != null ? $"{t.Teacher.FirstName} {t.Teacher.LastName}".Trim() : string.Empty,
                Day = t.DayOfWeek.ToString(),
                DayOfWeek = t.DayOfWeek,
                StartTime = t.StartTime.ToString("hh:mm tt"),
                EndTime = t.EndTime.ToString("hh:mm tt"),
                Duration = durationStr,
                RoomNumber = t.RoomNumber
            };
        }
    }
}
