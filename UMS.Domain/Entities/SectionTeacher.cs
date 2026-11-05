namespace UMS.Domain.Entities
{
    public class SectionTeacher
    {
        public Guid SectionId { get; set; }
        public Guid TeacherId { get; set; }

        public Section Section { get; set; } = null!;
        public Teacher Teacher { get; set; } = null!;
    }
}