using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.GymAdmins;

public interface IGymAdminRepository : IPendingRepository<GymAdmin, GymAdminId>
{
    Task<PagedResult<GymAdmin>> GetAll(PagedQuery query, CancellationToken ct);
}
