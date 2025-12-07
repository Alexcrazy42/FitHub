using FitHub.Application.Common;
using FitHub.Application.Files;
using FitHub.Application.Trainings.TrainingTypes;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Trainings.BaseGroupTrainings;
using FitHub.Domain.Files;
using FitHub.Domain.Trainings;
using FitHub.Shared.Common;

namespace FitHub.Application.Trainings.BaseGroupTrainings;

public class BaseGroupTrainingService : IBaseGroupTrainingService
{
    private readonly IBaseGroupTrainingRepository baseGroupTrainingRepository;
    private readonly ITrainingTypeRepository trainingTypeRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileService fileService;
    private readonly IFileRepository fileRepository;
    private readonly IBaseGroupTrainingPhotoRepository photoRepository;

    public BaseGroupTrainingService(IBaseGroupTrainingRepository baseGroupTrainingRepository,
        ITrainingTypeRepository trainingTypeRepository,
        IUnitOfWork unitOfWork,
        IFileService fileService,
        IBaseGroupTrainingPhotoRepository photoRepository,
        IFileRepository fileRepository)
    {
        this.baseGroupTrainingRepository = baseGroupTrainingRepository;
        this.trainingTypeRepository = trainingTypeRepository;
        this.unitOfWork = unitOfWork;
        this.fileService = fileService;
        this.photoRepository = photoRepository;
        this.fileRepository = fileRepository;
    }

    public Task<PagedResult<BaseGroupTraining>> GetAsync(PagedQuery pagedQuery, CancellationToken ct = default)
    {
        return baseGroupTrainingRepository.GetAsync(pagedQuery, ct);
    }

    public Task<BaseGroupTraining> GetByIdAsync(BaseGroupTrainingId id, CancellationToken ct = default)
    {
        return baseGroupTrainingRepository.GetById(id, ct);
    }

    public async Task<BaseGroupTraining> CreateAsync(CreateBaseGroupTrainingRequest request, CancellationToken ct)
    {
        var trainingTypeIds = request.TrainingTypeIds.Select(TrainingTypeId.Parse).ToList();
        var trainingTypes = await trainingTypeRepository.GetAsync(trainingTypeIds, ct);

        var baseGroupTraining = BaseGroupTraining.Create(
            name: ValidationException.ThrowIfNull(request.Name, "Имя не задано!"),
            description: ValidationException.ThrowIfNull(request.Description, "Описание не задано!"),
            durationInMinutes: ValidationException.ThrowIfNull(request.DurationInMinutes, "Не задана продолжительность!"),
            complexity: ValidationException.ThrowIfNull(request.Complexity, "Не задана сложность!"),
            isActive: ValidationException.ThrowIfNull(request.IsActive, "Не задана активность!")
        );
        baseGroupTraining.SetTrainingTypes(trainingTypes.ToList());
        await baseGroupTrainingRepository.PendingAddAsync(baseGroupTraining, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return baseGroupTraining;
    }

    public async Task<BaseGroupTraining> UpdateAsync(BaseGroupTrainingId id, CreateBaseGroupTrainingRequest request, CancellationToken ct)
    {
        var trainingTypeIds = request.TrainingTypeIds.Select(TrainingTypeId.Parse).ToList();
        var trainingTypes = await trainingTypeRepository.GetAsync(trainingTypeIds, ct);
        var entity = await GetByIdAsync(id, ct);
        ApplyChanges(entity, request, trainingTypes.ToList());
        await unitOfWork.SaveChangesAsync(ct);
        return entity;
    }

    public async Task AttachPhotosAsync(AttachPhotosRequest request, CancellationToken ct = default)
    {
        var trainingId = BaseGroupTrainingId.Parse(request.BaseGroupTrainingId);
        var training = await GetByIdAsync(trainingId, ct);
        ValidateAbilityAddMorePhotos(training);

        var fileId = FileId.Parse(request.FileId);
        var file = await fileService.GetFile(fileId, ct);

        var photo = BaseGroupTrainingPhoto.Create(training, file);

        file.SetEntity(trainingId.ToString(), EntityType.BaseGroupTraining);

        await photoRepository.PendingAddAsync(photo, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeattachPhotoAsync(FileId fileId, CancellationToken ct = default)
    {
        var file = await fileService.GetFile(fileId, ct);

        var photo = await photoRepository.GetFirstOrDefaultAsync(x => x.FileId == file.Id, ct);
        NotFoundException.ThrowIfNull(photo, "Фото не найдено!");

        fileRepository.PendingRemove(file);
        photoRepository.PendingRemove(photo);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(BaseGroupTrainingId id, CancellationToken ct)
    {
        var entity = await GetByIdAsync(id, ct);
        entity.SetIsDeleted(true);
        entity.SetTrainingTypes([]);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private void ApplyChanges(BaseGroupTraining entity, CreateBaseGroupTrainingRequest request, List<TrainingType> trainingTypes)
    {
        entity.SetName(ValidationException.ThrowIfNull(request.Name));
        entity.SetDescription(ValidationException.ThrowIfNull(request.Description));
        entity.SetDurationInMinutes(ValidationException.ThrowIfNull(request.DurationInMinutes));
        entity.SetComplexity(ValidationException.ThrowIfNull(request.Complexity));
        entity.SetTrainingTypes(trainingTypes);
        entity.SetIsActive(ValidationException.ThrowIfNull(request.IsActive));
    }

    private void ValidateAbilityAddMorePhotos(BaseGroupTraining training)
    {
        var entity = EntityExtension.GetAll().First(x => x.EntityType == EntityType.BaseGroupTraining);
        if (training.Photos.Count >= entity.MaxFileCount)
        {
            throw new ValidationException($"Нельзя добавить больше {entity.MaxFileCount} фотографий!");
        }
    }
}
