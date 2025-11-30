using FitHub.Application.Common;
using FitHub.Contracts.V1.Trainings.GroupTrainings;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.GroupTrainings;

public interface IGroupTrainingService
{
    Task<GroupTraining> GetByIdAsync(GroupTrainingId id, CancellationToken ct);

    Task<PagedResult<GroupTraining>> GetAsync(PagedQuery query, CancellationToken ct);

    Task<GroupTraining> CreateGroupTraining(AddOrUpdateGroupTrainingRequest request, CancellationToken ct);

    Task<GroupTraining> UpdateGroupTraining(GroupTrainingId id, AddOrUpdateGroupTrainingRequest request, CancellationToken ct);

    Task DeleteAsync(GroupTrainingId id, CancellationToken ct);
}
