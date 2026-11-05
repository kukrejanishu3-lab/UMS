
namespace UMS.Application.DTOs.Semester
{
    public class CreateSemesterDto
    {
        public string Name { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}
