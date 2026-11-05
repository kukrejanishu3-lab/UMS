namespace UMS.Application.DTOs.Semester
{
    // Information of a student evaluated and promoted during semester closing
    public class PromotedStudentDto
    {
        public Guid StudentId { get; set; }
        public string StudentRollNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public int PreviousSemester { get; set; }
        public int NewSemester { get; set; }
        public decimal SemesterSGPA { get; set; }
        public decimal CumulativeCGPA { get; set; }
        public string AcademicStanding { get; set; } = string.Empty;
        public string PromotionStatus { get; set; } = string.Empty;
    }

    // A course that a student failed (F) or dropped (W) and must retake
    public class RepeaterCourseDto
    {
        public Guid StudentId { get; set; }
        public string StudentRollNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string RequirementType { get; set; } = string.Empty; // "Failed (F) - Mandatory Repeat", "Dropped (W) - Registration Required"
    }

    // Comprehensive report returned after closing a semester and running promotion engine
    public class CloseSemesterResultDto
    {
        public Guid SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public DateOnly ClosedDate { get; set; }
        public int TotalStudentsEvaluated { get; set; }
        public int TotalPromoted { get; set; }
        public int TotalOnProbation { get; set; }
        public int TotalRepeatersFlagged { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<PromotedStudentDto> PromotedStudents { get; set; } = new();
        public List<PromotedStudentDto> ProbationStudents { get; set; } = new();
        public List<RepeaterCourseDto> RepeatRequiredCourses { get; set; } = new();
    }
}

