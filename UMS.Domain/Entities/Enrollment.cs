namespace UMS.Domain.Entities
{
    public class Enrollment
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid SectionId { get; set; }
        public DateOnly EnrollmentDate { get; set; }
        public string Status { get; set; } = "Enrolled";   // Enrolled, Dropped

        public Student Student { get; set; } = null!;
        public Section Section { get; set; } = null!;

        public decimal? Marks { get; set; }
        public string? Grade { get; set; }
        public decimal? GradePoints { get; set; }
    }
}