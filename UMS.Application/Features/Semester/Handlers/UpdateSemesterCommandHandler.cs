using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Semester;
using UMS.Application.Features.Semester.Commands;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Semester.Handlers
{
    // Handles updating an existing semester.
    public class UpdateSemesterCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<UpdateSemesterCommand, SemesterDto?>
    {
        public async Task<SemesterDto?> Handle(
            UpdateSemesterCommand request,
            CancellationToken cancellationToken)
        {
            var existingSemester = await dbContext.Semesters
                .FirstOrDefaultAsync(s => s.Id == request.SemesterId, cancellationToken);

            if (existingSemester is null)
            {
                return null;
            }

            existingSemester.Name = request.SemesterDto.Name;
            existingSemester.StartDate = request.SemesterDto.StartDate;
            existingSemester.EndDate = request.SemesterDto.EndDate;
            existingSemester.IsActive = request.SemesterDto.IsActive;

            await dbContext.SaveChangesAsync(cancellationToken);

            return new SemesterDto
            {
                Id = existingSemester.Id,
                Name = existingSemester.Name,
                StartDate = existingSemester.StartDate,
                EndDate = existingSemester.EndDate,
                IsActive = existingSemester.IsActive
            };
        }
    }
}
