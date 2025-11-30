using FitHub.Application.Common;
using FitHub.Application.Files;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Domain.Equipments;
using FitHub.Domain.Files;

namespace FitHub.Application.Equipments.Gyms;

internal sealed class GymService : IGymService
{
    private readonly IGymRepository gymRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileRepository fileRepository;
    private readonly IFileService fileService;

    public GymService(IGymRepository gymRepository, IUnitOfWork unitOfWork, IFileRepository fileRepository, IFileService fileService)
    {
        this.gymRepository = gymRepository;
        this.unitOfWork = unitOfWork;
        this.fileRepository = fileRepository;
        this.fileService = fileService;
    }

    public Task<PagedResult<Gym>> GetGymsAsync(PagedQuery pagedQuery, CancellationToken ct)
    {
        return gymRepository.GetGymsAsync(pagedQuery, ct);
    }

    public async Task<Gym> GetByIdAsync(GymId id, CancellationToken ct = default)
    {
        var gym = await gymRepository.GetSingleOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(gym, "Зал не найден!");
        var files = await fileRepository.GetAllAsync(x => x.EntityId == gym.Id.ToString() && x.EntityType == EntityType.Gym, ct);
        gym.SetFiles(files);
        return gym;
    }

    public async Task<Gym> CreateGymAsync(CreateGymRequest request, CancellationToken ct = default)
    {
        var gym = Gym.Create(request.Name.ValidateForNull(), request.Description.ValidateForNull());
        await gymRepository.PendingAddAsync(gym, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return gym;
    }

    public async Task<Gym> UpdateGymAsync(UpdateGymRequest request, CancellationToken ct = default)
    {
        var id = GymId.Parse(request.Id.ValidateForNull());
        var gym = await gymRepository.GetSingleOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(gym, "Зал не найден!");
        ApplyUpdateRequest(gym, request);
        await unitOfWork.SaveChangesAsync(ct);
        return gym;
    }

    public async Task DeleteAsync(GymId id, CancellationToken ct = default)
    {
        var gym = await gymRepository.GetSingleOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(gym, "Зал не найден!");
        gymRepository.PendingRemove(gym);
        await fileService.MakeFileNotActivePendingAsync(EntityType.Gym, gym.Id.ToString(), ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private void ApplyUpdateRequest(Gym gym, UpdateGymRequest request)
    {
        gym.UpdateName(request.Name.ValidateForNull());
        gym.UpdateDescription(request.Description.ValidateForNull());
    }
}
