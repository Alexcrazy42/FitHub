using FitHub.Application.Common;
using FitHub.Application.Trainings.BaseGroupTrainings;
using FitHub.Common.Entities;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Trainings.BaseGroupTrainings;

public class BaseGroupTrainingRepository : DefaultPendingRepository<BaseGroupTraining, BaseGroupTrainingId, DataContext>, IBaseGroupTrainingRepository
{
    private readonly DataContext context;

    public BaseGroupTrainingRepository(DataContext context) : base(context)
    {
        this.context = context;
    }

    public async Task<PagedResult<BaseGroupTraining>> GetAsync(PagedQuery query, CancellationToken ct)
    {
        var dbQuery = ReadRaw()
            .Include(x => x.TrainingTypes)
            .Where(x => !x.IsDeleted);

        var totalCount = await dbQuery.CountAsync(ct);

        var items = await dbQuery.ToListAsync(ct);
        return PagedResult<BaseGroupTraining>.Create(items, totalItems: totalCount, currentPage: query.PageNumber, pageSize: query.PageSize);
    }

    public async Task<BaseGroupTraining> GetById(BaseGroupTrainingId id, CancellationToken ct)
    {
        var entity = await ReadRaw()
            .Include(x => x.TrainingTypes)
            .Where(x => !x.IsDeleted)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        NotFoundException.ThrowIfNull(entity, "Базовая групповая тренировка не найдена!");
        return entity;
    }
}
