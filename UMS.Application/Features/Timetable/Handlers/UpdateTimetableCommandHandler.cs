using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Timetable;
using UMS.Application.Features.Timetable.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Timetable.Handlers
{
    // Handles updating an existing timetable entry.
    public class UpdateTimetableCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<UpdateTimetableCommand, TimetableDto?>
    {
        public async Task<TimetableDto?> Handle(
            UpdateTimetableCommand request,
            CancellationToken cancellationToken)
        {
            var existing = await dbContext.Timetables
                .FirstOrDefaultAsync(t => t.Id == request.TimetableId, cancellationToken);

            if (existing is null)
            {
                return null;
            }

            if (request.StartTime >= request.EndTime)
            {
                throw new ArgumentException("StartTime must be earlier than EndTime.");
            }

            // Fetch other schedules on the same day excluding this record
            var daySchedules = await dbContext.Timetables
                .AsNoTracking()
                .Where(t => t.Id != request.TimetableId && t.DayOfWeek == request.DayOfWeek)
                .Include(t => t.Teacher)
                .Include(t => t.Section)
                .ToListAsync(cancellationToken);

            // 1. Teacher Clash Check
            var teacherClash = daySchedules.FirstOrDefault(t =>
                t.TeacherId == request.TeacherId &&
                t.StartTime < request.EndTime && request.StartTime < t.EndTime);

            if (teacherClash != null)
            {
                var teacherName = teacherClash.Teacher != null
                    ? $"{teacherClash.Teacher.FirstName} {teacherClash.Teacher.LastName}".Trim()
                    : "This teacher";

                throw new InvalidOperationException(
                    $"Teacher conflict: {teacherName} already has a lecture on {request.DayOfWeek} from {teacherClash.StartTime:hh\\:mm tt} to {teacherClash.EndTime:hh\\:mm tt} in {teacherClash.RoomNumber}.");
            }

            // 2. Room Clash Check
            var roomClash = daySchedules.FirstOrDefault(t =>
                string.Equals(t.RoomNumber.Trim(), request.RoomNumber.Trim(), StringComparison.OrdinalIgnoreCase) &&
                t.StartTime < request.EndTime && request.StartTime < t.EndTime);

            if (roomClash != null)
            {
                throw new InvalidOperationException(
                    $"Room conflict: Room '{request.RoomNumber}' is already occupied on {request.DayOfWeek} from {roomClash.StartTime:hh\\:mm tt} to {roomClash.EndTime:hh\\:mm tt}.");
            }

            // 3. Section Clash Check
            var sectionClash = daySchedules.FirstOrDefault(t =>
                t.SectionId == request.SectionId &&
                t.StartTime < request.EndTime && request.StartTime < t.EndTime);

            if (sectionClash != null)
            {
                var sectionName = sectionClash.Section?.Name ?? "This section";
                throw new InvalidOperationException(
                    $"Section conflict: Section '{sectionName}' already has another class scheduled on {request.DayOfWeek} from {sectionClash.StartTime:hh\\:mm tt} to {sectionClash.EndTime:hh\\:mm tt}.");
            }

            existing.SectionId = request.SectionId;
            existing.TeacherId = request.TeacherId;
            existing.DayOfWeek = request.DayOfWeek;
            existing.StartTime = request.StartTime;
            existing.EndTime = request.EndTime;
            existing.RoomNumber = request.RoomNumber;

            await dbContext.SaveChangesAsync(cancellationToken);

            var teacher = await dbContext.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == existing.TeacherId, cancellationToken);

            var section = await dbContext.Sections
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == existing.SectionId, cancellationToken);

            var duration = existing.EndTime - existing.StartTime;
            var durationStr = duration.TotalMinutes >= 60
                ? $"{(int)duration.TotalHours} hr {(duration.Minutes > 0 ? duration.Minutes + " mins" : "")}".Trim()
                : $"{(int)duration.TotalMinutes} mins";

            return new TimetableDto
            {
                Id = existing.Id,
                SectionId = existing.SectionId,
                SectionName = section?.Name ?? string.Empty,
                TeacherId = existing.TeacherId,
                TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}".Trim() : string.Empty,
                Day = existing.DayOfWeek.ToString(),
                DayOfWeek = existing.DayOfWeek,
                StartTime = existing.StartTime.ToString("hh:mm tt"),
                EndTime = existing.EndTime.ToString("hh:mm tt"),
                Duration = durationStr,
                RoomNumber = existing.RoomNumber
            };
        }
    }
}
