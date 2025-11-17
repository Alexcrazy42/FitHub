using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.Trainers;

public class TrainerService : ITrainerService
{
    private readonly ITrainerRepository trainerRepository;
    private readonly IUnitOfWork unitOfWork;

    public TrainerService(ITrainerRepository trainerRepository, IUnitOfWork unitOfWork)
    {
        this.trainerRepository = trainerRepository;
        this.unitOfWork = unitOfWork;
    }

    public Task<PagedResult<Trainer>> GetAll(PagedQuery query, CancellationToken ct)
    {
        return trainerRepository.GetAll(query, ct);
    }

    public async Task SetStatus(TrainerId id, bool status, CancellationToken ct)
    {
        var trainer = await trainerRepository.GetAsync(id, ct);
        trainer.User.SetActive(status);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
