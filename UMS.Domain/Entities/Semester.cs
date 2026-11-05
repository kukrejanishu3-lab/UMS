namespace UMS.Domain.Entities
{
    public class Semester
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public bool IsActive { get; set; }

        // Marks if semester is concluded, results locked, and students promoted
        public bool IsClosed { get; set; } = false;
    }
}
