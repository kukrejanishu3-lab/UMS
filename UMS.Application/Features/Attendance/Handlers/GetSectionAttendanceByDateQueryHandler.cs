using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Attendance;
using UMS.Application.Features.Attendance.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Attendance.Handlers
{
    public class GetSectionAttendanceByDateQueryHandler : IRequestHandler<GetSectionAttendanceByDateQuery, SectionAttendanceSheetDto>
    {
        private readonly IAppDbContext _context;

        public GetSectionAttendanceByDateQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<SectionAttendanceSheetDto> Handle(GetSectionAttendanceByDateQuery request, CancellationToken cancellationToken)
        {
            var section = await _context.Sections
                .Include(s => s.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken);

            if (section is null)
            {
                throw new KeyNotFoundException("Section not found.");
            }

            var query = _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Timetable)
                .AsNoTracking()
                .Where(a => a.SectionId == request.SectionId && a.Date == request.Date);

            if (request.TimetableId.HasValue)
            {
                query = query.Where(a => a.TimetableId == request.TimetableId.Value);
            }

            var records = await query
                .OrderBy(a => a.Student.StudentNumber)
                .Select(a => new StudentAttendanceRecordDto
                {
                    AttendanceId = a.Id,
                    StudentId = a.StudentId,
                    StudentRollNo = a.Student.StudentNumber,
                    StudentName = $"{a.Student.FirstName} {a.Student.LastName}".Trim(),
                    TimetableId = a.TimetableId,
                    TimeSlot = a.Timetable != null ? $"{a.Timetable.StartTime:hh\\:mm} - {a.Timetable.EndTime:hh\\:mm}" : null,
                    Status = a.Status,
                    Remarks = a.Remarks
                })
                .ToListAsync(cancellationToken);

            string? slotSummary = null;
            if (request.TimetableId.HasValue)
            {
                var tt = await _context.Timetables.AsNoTracking().FirstOrDefaultAsync(t => t.Id == request.TimetableId.Value, cancellationToken);
                if (tt != null)
                {
                    slotSummary = $"{tt.StartTime:hh\\:mm} - {tt.EndTime:hh\\:mm} ({tt.DayOfWeek})";
                }
            }

            return new SectionAttendanceSheetDto
            {
                SectionId = section.Id,
                SectionName = section.Name,
                CourseName = section.Course.Name,
                Date = request.Date,
                TimetableId = request.TimetableId,
                TimeSlot = slotSummary,
                Records = records
            };
        }
    }
}
