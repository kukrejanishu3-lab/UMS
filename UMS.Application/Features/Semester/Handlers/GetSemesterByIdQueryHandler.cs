using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Semester;
using UMS.Application.Features.Semester.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Semester.Handlers
{
    // Handles retrieving a semester by ID.
    public class GetSemesterByIdQueryHandler(IAppDbContext dbContext)
        : IRequestHandler<GetSemesterByIdQuery, SemesterDto?>
    {
        public async Task<SemesterDto?> Handle(
            GetSemesterByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await dbContext.Semesters
                .AsNoTracking()
                .Where(s => s.Id == request.SemesterId)
                .Select(s => new SemesterDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsActive = s.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
