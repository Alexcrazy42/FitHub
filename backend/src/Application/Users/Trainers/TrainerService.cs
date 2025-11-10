using FitHub.Application.Common;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.Trainers;

public class TrainerService : ITrainerService
{
    private readonly ITrainerRepository trainerRepository;

    public TrainerService(ITrainerRepository trainerRepository)
    {
        this.trainerRepository = trainerRepository;
    }

    public Task<PagedResult<Trainer>> GetAll(PagedQuery query, CancellationToken ct)
    {
        return trainerRepository.GetAll(query, ct);
    }
}
