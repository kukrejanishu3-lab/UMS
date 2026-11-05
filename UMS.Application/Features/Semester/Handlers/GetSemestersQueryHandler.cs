using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Semester;
using UMS.Application.Features.Semester.Queries;
using UMS.Application.Interfaces;

namespace UMS.Application.Features.Semester.Handlers
{
    // Handles retrieving all semesters.
    public class GetSemestersQueryHandler(IAppDbContext dbContext)
        : IRequestHandler<GetSemestersQuery, IEnumerable<SemesterDto>>
    {
        public async Task<IEnumerable<SemesterDto>> Handle(
            GetSemestersQuery request,
            CancellationToken cancellationToken)
        {
            return await dbContext.Semesters
                .AsNoTracking()
                .Select(s => new SemesterDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsActive = s.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }
}
