namespace UMS.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int CreditHours { get; set; }

        public Guid DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
    }
}