using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Gyms;

public interface IGymZoneService
{
    Task<GymZone> CreateGymZoneAsync(CreateGymZoneRequest request, CancellationToken ct);
    Task<GymZone> UpdateGymZoneAsync(UpdateGymZoneRequest request, CancellationToken ct);
}
