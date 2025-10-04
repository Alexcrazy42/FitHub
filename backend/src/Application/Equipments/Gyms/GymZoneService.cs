using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Gyms;

public class GymZoneService : IGymZoneService
{
    private readonly IGymZoneRepository gymZoneRepository;
    private readonly IUnitOfWork unitOfWork;

    public GymZoneService(IGymZoneRepository gymZoneRepository, IUnitOfWork unitOfWork)
    {
        this.gymZoneRepository = gymZoneRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<GymZone> CreateGymZoneAsync(CreateGymZoneRequest request, CancellationToken ct)
    {
        var gymZone = GymZone.Create(request.Name.ValidateForNull(), request.Description.ValidateForNull());
        await gymZoneRepository.PendingAddAsync(gymZone, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return gymZone;
    }

    public async Task<GymZone> UpdateGymZoneAsync(UpdateGymZoneRequest request, CancellationToken ct)
    {
        var gymZone = await gymZoneRepository.GetSingleOrDefaultAsync(x => x.Id == GymZoneId.Parse(request.Id), ct);
        NotFoundException.ThrowIfNull(gymZone, "Зона зала не найдена!");
        ApplyUpdateRequest(gymZone, request);
        await unitOfWork.SaveChangesAsync(ct);
        return gymZone;
    }

    public async Task DeleteAsync(GymZoneId id, CancellationToken ct)
    {
        var gymZone = await gymZoneRepository.GetSingleOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(gymZone, "Зона зала не найдена!");
        gymZoneRepository.PendingRemove(gymZone, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private void ApplyUpdateRequest(GymZone gymZone, UpdateGymZoneRequest request)
    {
        gymZone.Name = request.Name.ValidateForNull();
        gymZone.Description = request.Description.ValidateForNull();
    }
}
