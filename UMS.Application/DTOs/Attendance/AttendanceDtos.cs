namespace UMS.Application.DTOs.Attendance
{
    // DTO received when marking bulk attendance for a section
    public class MarkAttendanceDto
    {
        public DateOnly Date { get; set; }
        public Guid? TimetableId { get; set; } // Optional link to specific timetable slot
        public List<StudentAttendanceItemDto> Attendances { get; set; } = new();
    }

    // Individual student attendance entry
    public class StudentAttendanceItemDto
    {
        public Guid StudentId { get; set; }
        public string Status { get; set; } = "Present"; // Present, Absent, Late, Excused
        public string? Remarks { get; set; }
    }

    // Result summary returned after marking attendance
    public class MarkAttendanceResultDto
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public Guid? TimetableId { get; set; }
        public string? TimeSlot { get; set; }
        public int TotalMarked { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int ExcusedCount { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<StudentAttendanceRecordDto> Records { get; set; } = new();
    }

    // Course-wise attendance summary for a student with 75% rule
    public class StudentCourseAttendanceDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int TotalLectures { get; set; }
        public int AttendedLectures { get; set; }
        public int AbsentLectures { get; set; }
        public decimal AttendancePercentage { get; set; }
        public bool IsShortAttendance { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
    }

    // Detailed sheet of a section's attendance on a specific date
    public class SectionAttendanceSheetDto
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public Guid? TimetableId { get; set; }
        public string? TimeSlot { get; set; }
        public List<StudentAttendanceRecordDto> Records { get; set; } = new();
    }

    public class StudentAttendanceRecordDto
    {
        public Guid AttendanceId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentRollNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public Guid? TimetableId { get; set; }
        public string? TimeSlot { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    // DTO to update an existing individual attendance record
    public class UpdateAttendanceDto
    {
        public string Status { get; set; } = "Present"; // Present, Absent, Late, Excused
        public string? Remarks { get; set; }
    }
}
