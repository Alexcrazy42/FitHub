using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.Trainers;

public interface ITrainerRepository : IPendingRepository<Trainer, TrainerId>
{
    Task<PagedResult<Trainer>> GetAll(PagedQuery query, CancellationToken ct);

    Task<Trainer> GetAsync(TrainerId id, CancellationToken ct);
}
