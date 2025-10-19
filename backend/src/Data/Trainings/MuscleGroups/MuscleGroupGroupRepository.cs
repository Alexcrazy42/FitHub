using FitHub.Application.Common;
using FitHub.Application.Trainings.MuscleGroups;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Trainings.MuscleGroups;

public class MuscleGroupRepository : DefaultPendingRepository<MuscleGroup, MuscleGroupId, DataContext>, IMuscleGroupRepository
{
    public MuscleGroupRepository(DataContext context) : base(context)
    {
    }

    public async Task<PagedResult<MuscleGroup>> GetAll(PagedQuery pagedQuery, CancellationToken ct)
    {
        var dbQuery = ReadRaw();

        var totalItems = await dbQuery.CountAsync(ct);

        var items = await dbQuery
            .Skip((pagedQuery.PageNumber - 1) * pagedQuery.PageSize)
            .Take(pagedQuery.PageSize)
            .ToListAsync(ct);

        return PagedResult<MuscleGroup>.Create(items: items, totalItems: totalItems, currentPage: pagedQuery.PageNumber, pageSize: pagedQuery.PageSize);
    }
}
