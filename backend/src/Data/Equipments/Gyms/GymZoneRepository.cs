using FitHub.Application.Equipments.Gyms;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Equipments;

namespace FitHub.Data.Equipments.Gyms;

public class GymZoneRepository : DefaultPendingRepository<GymZone, GymZoneId, DataContext>, IGymZoneRepository
{
    public GymZoneRepository(DataContext context) : base(context)
    {
    }
}
