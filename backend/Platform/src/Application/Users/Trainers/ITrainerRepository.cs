using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Users.Trainers;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.Trainers;

public interface ITrainerRepository : IPendingRepository<Trainer, TrainerId>
{
    Task<PagedResult<Trainer>> GetAll(PagedQuery query, TrainerQuery? trainerQuery, CancellationToken ct);

    Task<Trainer> GetAsync(TrainerId id, CancellationToken ct);
    Task<Trainer> GetByUserId(IdentityUserId userId, CancellationToken ct);
}
