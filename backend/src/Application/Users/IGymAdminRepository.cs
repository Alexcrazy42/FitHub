using FitHub.Common.Entities.Storage;
using FitHub.Domain.Users;

namespace FitHub.Application.Users;

public interface IGymAdminRepository : IPendingRepository<GymAdmin, GymAdminId>
{
}
