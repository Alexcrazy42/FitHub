using FitHub.Application.Trainings.BaseGroupTrainings;
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

    public Task<BaseGroupTraining?> GetOrDefaultAsync(BaseGroupTrainingId id, CancellationToken ct = default)
    {
        return context.BaseGroupTrainings
            .Include(x => x.Type)
            .SingleOrDefaultAsync(x => x.Id == id, ct);
    }
}
