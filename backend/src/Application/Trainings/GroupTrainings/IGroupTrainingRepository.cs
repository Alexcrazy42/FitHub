using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.GroupTrainings;

public interface IGroupTrainingRepository : IPendingRepository<GroupTraining, GroupTrainingId>
{
    Task<PagedResult<GroupTraining>> GetAsync(PagedQuery query, CancellationToken ct);

    Task<GroupTraining> GetAsync(GroupTrainingId groupTrainingId, CancellationToken ct);
}
