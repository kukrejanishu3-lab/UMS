namespace UMS.Domain.Entities
{
    public class DropCourseRequest
    {
        public Guid Id { get; set; }
        public Guid EnrollmentId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";   // Pending, Approved, Rejected
        public DateTime RequestDate { get; set; }
        public string? AdminRemarks { get; set; }

        public Enrollment Enrollment { get; set; } = null!;
    }
}