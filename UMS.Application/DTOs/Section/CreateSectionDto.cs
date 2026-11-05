namespace UMS.Application.DTOs.Section
{
    public class CreateSectionDto
    {
        public string Name { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public Guid CourseId { get; set; }

        public Guid SemesterId { get; set; }
    }
}