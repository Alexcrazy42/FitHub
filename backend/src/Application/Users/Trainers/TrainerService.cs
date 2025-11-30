using FitHub.Application.Common;
using FitHub.Application.Trainings.GroupTrainings;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Users.Trainers;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.Trainers;

public class TrainerService : ITrainerService
{
    private readonly ITrainerRepository trainerRepository;
    private readonly IGroupTrainingRepository groupTrainingRepository;
    private readonly IUnitOfWork unitOfWork;

    public TrainerService(ITrainerRepository trainerRepository, IUnitOfWork unitOfWork, IGroupTrainingRepository groupTrainingRepository)
    {
        this.trainerRepository = trainerRepository;
        this.unitOfWork = unitOfWork;
        this.groupTrainingRepository = groupTrainingRepository;
    }

    public Task<PagedResult<Trainer>> GetAll(PagedQuery query, TrainerQuery? trainerQuery, CancellationToken ct)
    {
        return trainerRepository.GetAll(query, trainerQuery, ct);
    }

    public async Task SetStatus(TrainerId id, bool status, CancellationToken ct)
    {
        var trainer = await trainerRepository.GetAsync(id, ct);
        trainer.User.SetActive(status);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public Task<Trainer> GetByIdAsync(TrainerId id, CancellationToken ct)
    {
        return trainerRepository.GetAsync(id, ct);
    }

    public async Task<bool> IsAvailableAsync(Trainer trainer, DateTimeOffset start, DateTimeOffset end, CancellationToken ct)
    {
        var existingTraining = await groupTrainingRepository.GetFirstOrDefaultAsync(
            x => x.TrainerId == trainer.Id &&
                 x.StartTime < end &&
                 x.EndTime > start,
            ct);

        return existingTraining is null;
    }

    public Task<Trainer> GetByUserIdAsync(IdentityUserId userId, CancellationToken ct)
    {
        return trainerRepository.GetByUserId(userId, ct);
    }
}
