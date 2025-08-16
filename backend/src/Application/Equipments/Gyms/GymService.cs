using FitHub.Contracts.V1.Equipments;
using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Gyms;

internal sealed class GymService : IGymService
{
    private readonly IGymRepository gymRepository;

    public GymService(IGymRepository gymRepository)
    {
        this.gymRepository = gymRepository;
    }

    public Task<Gym?> GetGymOrDefaultAsync(GymId id, CancellationToken ct = default)
    {
        return gymRepository.GetSingleOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<Gym> CreateGymAsync(CreateGymRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Gym> UpdateGymAsync(UpdateGymRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
