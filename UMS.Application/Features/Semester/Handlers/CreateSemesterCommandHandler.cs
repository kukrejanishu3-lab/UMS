using MediatR;
using UMS.Application.DTOs.Semester;
using UMS.Application.Features.Semester.Commands;
using UMS.Application.Interfaces;
using SemesterEntity = UMS.Domain.Entities.Semester;

namespace UMS.Application.Features.Semester.Handlers
{
    // Handles creating a new semester.
    public class CreateSemesterCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<CreateSemesterCommand, SemesterDto>
    {
        public async Task<SemesterDto> Handle(
            CreateSemesterCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.SemesterDto;

            var semester = new SemesterEntity
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive
            };

            await dbContext.Semesters.AddAsync(semester, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new SemesterDto
            {
                Id = semester.Id,
                Name = semester.Name,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate,
                IsActive = semester.IsActive
            };
        }
    }
}
