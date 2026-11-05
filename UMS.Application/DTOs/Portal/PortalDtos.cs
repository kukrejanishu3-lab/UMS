namespace UMS.Application.DTOs.Portal
{
    public class StudentCourseDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string SemesterName { get; set; } = string.Empty;
        public string EnrollmentStatus { get; set; } = string.Empty;
        public string? Grade { get; set; }
        public decimal? GradePoints { get; set; }
    }

    public class StudentScheduleDto
    {
        public Guid TimetableId { get; set; }
        public string Day { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
    }

    public class TeacherSectionDto
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string SemesterName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int EnrolledStudentsCount { get; set; }
    }

    public class TeacherScheduleDto
    {
        public Guid TimetableId { get; set; }
        public string Day { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
    }

    public class SectionStudentRosterDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class StudentDropRequestDto
    {
        public Guid RequestId { get; set; }
        public Guid EnrollmentId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string SemesterName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string? AdminRemarks { get; set; }
        public string? Grade { get; set; }
    }
}

