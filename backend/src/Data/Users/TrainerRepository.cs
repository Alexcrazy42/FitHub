using FitHub.Application.Users;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Users;

namespace FitHub.Data.Users;

public class TrainerRepository :
    DefaultPendingRepository<Trainer, TrainerId, DataContext>,
    ITrainerRepository
{
    public TrainerRepository(DataContext context) : base(context)
    {
    }
}
