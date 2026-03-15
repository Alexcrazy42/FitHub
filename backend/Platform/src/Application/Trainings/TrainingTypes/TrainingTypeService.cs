using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Trainings.TrainingTypes;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.TrainingTypes;

public class TrainingTypeService : ITrainingTypeService
{
    private readonly ITrainingTypeRepository trainingTypeRepository;
    private readonly IUnitOfWork unitOfWork;

    public TrainingTypeService(ITrainingTypeRepository trainingTypeRepository, IUnitOfWork unitOfWork)
    {
        this.trainingTypeRepository = trainingTypeRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<TrainingType> CreateAsync(CreateTrainingTypeRequest request, CancellationToken ct)
    {
        var name = ValidationException.ThrowIfNull(request.Name, nameof(request.Name));
        var trainingType = TrainingType.Create(name);
        await trainingTypeRepository.PendingAddAsync(trainingType, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return trainingType;
    }

    public async Task<TrainingType> UpdateAsync(TrainingTypeId id, CreateTrainingTypeRequest request, CancellationToken ct)
    {
        var trainingType = await trainingTypeRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(trainingType, "Тип тренировки не найден!");
        trainingType.SetName(ValidationException.ThrowIfNull(request.Name, "Имя не может быть пустым!"));
        await unitOfWork.SaveChangesAsync(ct);
        return trainingType;
    }

    public async Task DeleteAsync(TrainingTypeId id, CancellationToken ct)
    {
        var trainingType = await trainingTypeRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(trainingType, "Тип тренировки не найден!");
        trainingTypeRepository.PendingRemove(trainingType);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
