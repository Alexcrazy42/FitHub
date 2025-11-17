using FitHub.Application.Common;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.Trainers;

public interface ITrainerService
{
    Task<PagedResult<Trainer>> GetAll(PagedQuery query, CancellationToken ct);

    Task SetStatus(TrainerId id, bool status, CancellationToken ct);
}
