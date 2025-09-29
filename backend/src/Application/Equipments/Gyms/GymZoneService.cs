using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Gyms;

public class GymZoneService : IGymZoneService
{
    private readonly IGymZoneRepository gymZoneRepository;

    public GymZoneService(IGymZoneRepository gymZoneRepository)
    {
        this.gymZoneRepository = gymZoneRepository;
    }

    public Task<GymZone> CreateGymZoneAsync(CreateGymZoneRequest request, CancellationToken ct) => throw new NotImplementedException();

    public Task<GymZone> UpdateGymZoneAsync(UpdateGymZoneRequest request, CancellationToken ct) => throw new NotImplementedException();
}
