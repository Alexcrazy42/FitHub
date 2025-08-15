using FitHub.Application.Equipments.Gyms;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Equipments;

namespace FitHub.Data.Equipments.Gyms;

public class GymRepository : DefaultPendingRepository<Gym, GymId, DataContext>, IGymRepository
{
    public GymRepository(DataContext context) : base(context)
    {
    }
}
