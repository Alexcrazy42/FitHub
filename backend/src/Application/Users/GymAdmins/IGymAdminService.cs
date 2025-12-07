using FitHub.Application.Common;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Domain.Equipments;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.GymAdmins;

public interface IGymAdminService
{
    Task<PagedResult<GymAdmin>> GetAll(PagedQuery query, CancellationToken ct);

    Task SetStatus(GymAdminId gymAdminId, bool status, CancellationToken ct);

    Task<GymAdmin> GetByUserIdAsync(IdentityUserId userId, CancellationToken ct);
}
