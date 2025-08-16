using FitHub.Contracts.V1.Equipments;
using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Gyms;

public interface IGymService
{
    Task<Gym?> GetGymOrDefaultAsync(GymId id, CancellationToken ct = default);

    Task<Gym> CreateGymAsync(CreateGymRequest request, CancellationToken ct = default);

    Task<Gym> UpdateGymAsync(UpdateGymRequest request, CancellationToken ct = default);
}
