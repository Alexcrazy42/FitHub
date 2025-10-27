using FitHub.Application.Trainings.TrainingTypes;
using FitHub.Common.Entities;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Trainings.TrainingTypes;

public class TrainingTypeRepository : DefaultPendingRepository<TrainingType, TrainingTypeId, DataContext>, ITrainingTypeRepository
{
    private readonly DataContext context;

    public TrainingTypeRepository(DataContext context) : base(context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyList<TrainingType>> GetAsync(IReadOnlyList<TrainingTypeId> ids, CancellationToken ct)
    {
        var items = await ReadRaw().Where(x => ids.Contains(x.Id)).ToListAsync(ct);
        if (items.Count != ids.Count)
        {
            throw new NotFoundException("Не все типы тренировок были найдены!");
        }

        return items;
    }
}
