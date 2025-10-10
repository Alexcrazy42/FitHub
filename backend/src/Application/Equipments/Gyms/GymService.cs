using FitHub.Application.Common;
using FitHub.Application.Files;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Gyms;

internal sealed class GymService : IGymService
{
    private readonly IGymRepository gymRepository;
    private readonly IFileService fileService;
    private readonly IUnitOfWork unitOfWork;

    public GymService(IGymRepository gymRepository, IUnitOfWork unitOfWork, IFileService fileService)
    {
        this.gymRepository = gymRepository;
        this.unitOfWork = unitOfWork;
        this.fileService = fileService;
    }

    public Task<PagedResult<Gym>> GetGymsAsync(PagedQuery pagedQuery, CancellationToken ct)
    {
        return gymRepository.GetGymsAsync(pagedQuery, ct);
    }

    public Task<Gym?> GetGymOrDefaultAsync(GymId id, CancellationToken ct = default)
    {
        return gymRepository.GetSingleOrDefaultAsync(x => x.Id == id, ct);
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

    public async Task<Gym> AddFileAsync(AddFileRequest request, CancellationToken ct)
    {
        var id = request.GymId.ValidateForNull("не передан id зала");
        var file = request.File.ValidateForNull("не передан файл");

        var gym = await gymRepository.GetSingleOrDefaultAsync(x => x.Id == GymId.Parse(id), ct);

        NotFoundException.ThrowIfNull(gym, "Зал не найден");

        if (gym.ImageRelativePath is not null && gym.InUploadProcess is null)
        {
            throw new AlreadyExistsException("Нельзя прикрепить фотографию, не удалив предыдущую");
        }

        var key = $"gym/{id}/" + Path.GetFileName(file.FileName);

        gym.InUploadProcess = true;
        gym.SetImageRelativePath(key);
        await unitOfWork.SaveChangesAsync(ct);

        await using var stream = file.OpenReadStream();
        
        await fileService.UploadFileAsync(key, stream, file.ContentType);

        gym.InUploadProcess = null;
        await unitOfWork.SaveChangesAsync(ct);
        return gym;
    }

    public async Task<Gym> RemoveFileAsync(GymId id, CancellationToken ct = default)
    {
        var gym = await gymRepository.GetSingleOrDefaultAsync(x => x.Id == id, ct);

        NotFoundException.ThrowIfNull(gym, "Зал не найден");

        if (gym.ImageRelativePath is null)
        {
            throw new NotFoundException("Фотография не найдена!");
        }

        await fileService.DeleteFileAsync(gym.ImageRelativePath);

        gym.SetImageRelativePath(null);

        await unitOfWork.SaveChangesAsync(ct);

        return gym;
    }

    private void ApplyUpdateRequest(Gym gym, UpdateGymRequest request)
    {
        gym.UpdateName(request.Name.ValidateForNull());
        gym.UpdateDescription(request.Description.ValidateForNull());
    }
}
