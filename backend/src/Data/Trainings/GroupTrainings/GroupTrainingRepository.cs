using FitHub.Application.Common;
using FitHub.Application.Trainings.GroupTrainings;
using FitHub.Common.Entities;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Trainings.GroupTrainings;

internal sealed class GroupTrainingRepository : DefaultPendingRepository<GroupTraining, GroupTrainingId, DataContext>, IGroupTrainingRepository
{
    public GroupTrainingRepository(DataContext context) : base(context)
    {
    }

    protected override IQueryable<GroupTraining> ReadRaw()
    {
        return base.ReadRaw()
            .Include(x => x.Gym)
            .Include(x => x.Trainer)
                .ThenInclude(x => x.User)
            .Include(x => x.Participants)
                .ThenInclude(x => x.User)
            .Include(x => x.BaseGroupTraining)
            .AsQueryable();
    }

    public async Task<PagedResult<GroupTraining>> GetAsync(PagedQuery query, CancellationToken ct)
    {
        var dbQuery = ReadRaw()
            .AsQueryable();

        var total = await dbQuery.CountAsync(ct);

        var items = await dbQuery
            .OrderBy(x => x.Id)
            .Skip(query.PageSize * (query.PageNumber - 1))
            .Take(query.PageSize)
            .ToReadOnlyListAsync(ct);

        return PagedResult<GroupTraining>.Create(items: items, totalItems: total, currentPage: query.PageNumber, pageSize: query.PageSize);
    }

    public async Task<GroupTraining> GetAsync(GroupTrainingId groupTrainingId, CancellationToken ct)
    {
        var entity = await ReadRaw()
            .FirstOrDefaultAsync(x => x.Id == groupTrainingId, ct);

        NotFoundException.ThrowIfNull(entity, "Тренировка не найдена!");

        return entity;
    }
}
