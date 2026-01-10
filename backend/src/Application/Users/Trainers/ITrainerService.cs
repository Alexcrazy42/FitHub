using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Contracts.V1.Users.Trainers;
using FitHub.Domain.Trainings;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.Trainers;

public interface ITrainerService
{
    Task<PagedResult<Trainer>> GetAll(PagedQuery query, TrainerQuery? trainerQuery, CancellationToken ct);

    Task SetStatus(TrainerId id, bool status, CancellationToken ct);

    Task<Trainer> GetByIdAsync(TrainerId id, CancellationToken ct);

    Task<bool> IsAvailableAsync(Trainer trainer, GroupTraining groupTraining, DateTimeOffset start, DateTimeOffset end, CancellationToken ct);

    Task<Trainer> GetByUserIdAsync(IdentityUserId userId, CancellationToken ct);
}
