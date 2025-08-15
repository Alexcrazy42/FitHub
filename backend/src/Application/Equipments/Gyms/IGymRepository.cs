using FitHub.Common.Entities.Storage;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Gyms;

public interface IGymRepository : IPendingRepository<Gym, GymId>
{

}
