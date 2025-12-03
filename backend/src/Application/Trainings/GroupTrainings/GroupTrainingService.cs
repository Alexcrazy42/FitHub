using FitHub.Application.Common;
using FitHub.Application.Equipments.Gyms;
using FitHub.Application.Trainings.BaseGroupTrainings;
using FitHub.Application.Users.Trainers;
using FitHub.Application.Users.Visitors;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Trainings.GroupTrainings;
using FitHub.Domain.Equipments;
using FitHub.Domain.Trainings;
using FitHub.Domain.Users;

namespace FitHub.Application.Trainings.GroupTrainings;

internal sealed class GroupTrainingService : IGroupTrainingService
{
    private readonly IGroupTrainingRepository groupTrainingRepository;
    private readonly IGymService gymService;
    private readonly ITrainerRepository trainerRepository;
    private readonly ITrainerService trainerService;
    private readonly IBaseGroupTrainingService baseGroupTrainingService;
    private readonly IVisitorService visitorService;
    private readonly IUnitOfWork unitOfWork;

    public GroupTrainingService(IGroupTrainingRepository groupTrainingRepository, IUnitOfWork unitOfWork, ITrainerRepository trainerRepository, IBaseGroupTrainingService baseGroupTrainingService, IVisitorService visitorService, ITrainerService trainerService, IGymService gymService)
    {
        this.groupTrainingRepository = groupTrainingRepository;
        this.unitOfWork = unitOfWork;
        this.trainerRepository = trainerRepository;
        this.baseGroupTrainingService = baseGroupTrainingService;
        this.visitorService = visitorService;
        this.trainerService = trainerService;
        this.gymService = gymService;
    }

    public Task<GroupTraining> GetByIdAsync(GroupTrainingId id, CancellationToken ct)
    {
        return groupTrainingRepository.GetAsync(id, ct);
    }

    public Task<PagedResult<GroupTraining>> GetAsync(PagedQuery query, CancellationToken ct)
    {
        return groupTrainingRepository.GetAsync(query, ct);
    }

    public async Task<GroupTraining> CreateGroupTraining(AddOrUpdateGroupTrainingRequest request, CancellationToken ct)
    {
        var startTime = ValidationException.ThrowIfNull(request.StartTime, "Укажите время начало тренировки!");
        var endTime = ValidationException.ThrowIfNull(request.EndTime, "Укажите время конца тренировки!");

        var gym = await gymService.GetByIdAsync(GymId.Parse(request.GymId), ct);

        var trainer = await trainerService.GetByIdAsync(TrainerId.Parse(request.TrainerId), ct);

        var baseGroupTraining = await baseGroupTrainingService.GetByIdAsync(BaseGroupTrainingId.Parse(request.BaseGroupTrainingId), ct);

        var groupTraining = GroupTraining.Create(baseGroupTraining, gym, trainer, startTime, endTime);

        await CheckTrainerAvailabilityAsync(trainer, groupTraining, startTime, endTime, ct);

        await groupTrainingRepository.PendingAddAsync(groupTraining, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return groupTraining;
    }

    public async Task<GroupTraining> UpdateGroupTraining(GroupTrainingId id, AddOrUpdateGroupTrainingRequest request, CancellationToken ct)
    {
        var training = await GetByIdAsync(id, ct);
        await UpdateAsync(training, request, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return training;
    }

    public async Task DeleteAsync(GroupTrainingId id, CancellationToken ct)
    {
        var entity = await GetByIdAsync(id, ct);
        groupTrainingRepository.PendingRemove(entity);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task UpdateAsync(GroupTraining training, AddOrUpdateGroupTrainingRequest request, CancellationToken ct)
    {
        if (request.IsActive.HasValue)
        {
            training.SetActive(request.IsActive.Value);
        }

        if (request is { StartTime: not null, EndTime: not null })
        {
            training.SetSchedule(request.StartTime.Value, request.EndTime.Value);
        }

        if (request.TrainerId is not null)
        {
            var trainerId = TrainerId.Parse(request.TrainerId);
            var trainer = await trainerService.GetByIdAsync(trainerId, ct);
            await CheckTrainerAvailabilityAsync(trainer, training, training.StartTime, training.EndTime, ct);
            training.SetTrainer(trainer);
        }

        await CheckTrainerAvailabilityAsync(training.Trainer, training, training.StartTime, training.EndTime, ct);

        if (request.BaseGroupTrainingId is not null)
        {
            var baseGroupTrainingId = BaseGroupTrainingId.Parse(request.BaseGroupTrainingId);
            var baseGroupTraining = await baseGroupTrainingService.GetByIdAsync(baseGroupTrainingId, ct);
            training.SetBaseGroupTraining(baseGroupTraining);
        }
    }

    private async Task CheckTrainerAvailabilityAsync(Trainer trainer,
        GroupTraining groupTraining,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        CancellationToken ct)
    {
        var isTrainerAvailable = await trainerService.IsAvailableAsync(trainer, groupTraining, startTime, endTime, ct);

        if (!isTrainerAvailable)
        {
            throw new ValidationException("Тренер занят в этот временной промежуток!");
        }
    }
}
