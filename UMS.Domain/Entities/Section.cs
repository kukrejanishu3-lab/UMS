namespace UMS.Domain.Entities
{
    public class Section
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public Guid CourseId { get; set; }

        public Guid SemesterId { get; set; }

        public Course Course { get; set; } = null!;

        public Semester Semester { get; set; } = null!;
    }
}