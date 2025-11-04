using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Gyms;

public interface IGymRepository : IPendingRepository<Gym, GymId>
{
    Task<PagedResult<Gym>> GetGymsAsync(PagedQuery pagedQuery, CancellationToken ct = default);

    Task<Gym> GetById(GymId id, CancellationToken ct);
}
