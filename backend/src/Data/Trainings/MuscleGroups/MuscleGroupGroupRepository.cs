using FitHub.Application.Common;
using FitHub.Application.Trainings.MuscleGroups;
using FitHub.Common.Entities;
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
        var dbQuery = ReadRaw()
            .Where(x => x.ParentId == null)
            .Include(x => x.Childrens)
            .Include(x => x.Parent);

        var totalItems = await dbQuery.CountAsync(ct);

        var items = await dbQuery
            .Skip((pagedQuery.PageNumber - 1) * pagedQuery.PageSize)
            .Take(pagedQuery.PageSize)
            .ToListAsync(ct);

        return PagedResult<MuscleGroup>.Create(items: items, totalItems: totalItems, currentPage: pagedQuery.PageNumber, pageSize: pagedQuery.PageSize);
    }

    public async Task<MuscleGroup> GetById(MuscleGroupId id, CancellationToken ct)
    {
        var muscleGroup = await ReadRaw()
            .Where(x => x.ParentId == null)
            .Include(x => x.Childrens)
            .Include(x => x.Parent)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        NotFoundException.ThrowIfNull(muscleGroup, "Группа мышц не найден!");
        return muscleGroup;
    }
}
