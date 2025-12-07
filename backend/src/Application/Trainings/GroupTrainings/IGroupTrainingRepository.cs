using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Trainings.GroupTrainings;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.GroupTrainings;

public interface IGroupTrainingRepository : IPendingRepository<GroupTraining, GroupTrainingId>
{
    Task<PagedResult<GroupTraining>> GetAsync(PagedQuery query, GroupTrainingSearchRequest? searchRequest, CancellationToken ct);

    Task<GroupTraining> GetAsync(GroupTrainingId groupTrainingId, CancellationToken ct);
}
