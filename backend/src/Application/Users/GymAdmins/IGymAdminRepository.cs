using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.GymAdmins;

public interface IGymAdminRepository : IPendingRepository<GymAdmin, GymAdminId>
{
    Task<PagedResult<GymAdmin>> GetAll(PagedQuery query, CancellationToken ct);

    Task<GymAdmin> GetAsync(GymAdminId id, CancellationToken ct);
    Task<GymAdmin> GetByUserIdAsync(IdentityUserId userId, CancellationToken ct);
}
