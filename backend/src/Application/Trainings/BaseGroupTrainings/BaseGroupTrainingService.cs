using FitHub.Application.Common;
using FitHub.Application.Trainings.TrainingTypes;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Trainings.BaseGroupTrainings;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.BaseGroupTrainings;

public class BaseGroupTrainingService : IBaseGroupTrainingService
{
    private readonly IBaseGroupTrainingRepository baseGroupTrainingRepository;
    private readonly ITrainingTypeRepository trainingTypeRepository;
    private readonly IUnitOfWork unitOfWork;

    public BaseGroupTrainingService(IBaseGroupTrainingRepository baseGroupTrainingRepository,
        ITrainingTypeRepository trainingTypeRepository,
        IUnitOfWork unitOfWork)
    {
        this.baseGroupTrainingRepository = baseGroupTrainingRepository;
        this.trainingTypeRepository = trainingTypeRepository;
        this.unitOfWork = unitOfWork;
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
}
