namespace UMS.Application.DTOs.Grading
{
    // Input for a single student's course marks
    public class StudentGradeInputDto
    {
        public Guid StudentId { get; set; }
        public decimal Marks { get; set; } // 0 to 100
        public string? Remarks { get; set; }
    }

    // Input DTO for teacher submitting grades for a section
    public class SubmitCourseGradesDto
    {
        public List<StudentGradeInputDto> Grades { get; set; } = new();
    }

    // Input DTO for updating a single student's course grade
    public class UpdateGradeDto
    {
        public decimal Marks { get; set; } // 0 to 100
        public string? Remarks { get; set; }
    }

    // Detailed grade entry for a student
    public class StudentGradeRecordDto
    {
        public Guid EnrollmentId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentRollNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public decimal? Marks { get; set; }
        public string? Grade { get; set; }
        public decimal? GradePoints { get; set; }
        public string? Remarks { get; set; }
    }

    // Result returned after teacher submits grades
    public class SubmitCourseGradesResultDto
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int TotalGraded { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<StudentGradeRecordDto> Grades { get; set; } = new();
    }

    // Section award list / grade sheet view
    public class SectionGradeSheetDto
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SemesterName { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public List<StudentGradeRecordDto> Grades { get; set; } = new();
    }

    // Course item in student transcript
    public class StudentTranscriptCourseDto
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public decimal? Marks { get; set; }
        public string? Grade { get; set; }
        public decimal? GradePoints { get; set; }
        public decimal QualityPoints { get; set; }
        public string Status { get; set; } = string.Empty; // Pass, Fail, Dropped
    }

    // Semester summary in student transcript (Includes both Semester SGPA & Progressive Cumulative CGPA)
    public class SemesterTranscriptDto
    {
        public Guid SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public int CreditsAttempted { get; set; }
        public int CreditsEarned { get; set; }
        public decimal SemesterQualityPoints { get; set; }
        public decimal SGPA { get; set; }
        public decimal CumulativeCGPA { get; set; } // Cumulative CGPA up to this semester
        public List<StudentTranscriptCourseDto> Courses { get; set; } = new();
    }

    // Full student digital transcript with CGPA and Academic Standing
    public class StudentTranscriptDto
    {
        public Guid StudentId { get; set; }
        public string StudentRollNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int CurrentSemester { get; set; }
        public int TotalCreditsAttempted { get; set; }
        public int TotalCreditsEarned { get; set; }
        public decimal TotalQualityPoints { get; set; }
        public decimal CGPA { get; set; }
        public string AcademicStanding { get; set; } = string.Empty;
        public List<SemesterTranscriptDto> Semesters { get; set; } = new();
    }
}
