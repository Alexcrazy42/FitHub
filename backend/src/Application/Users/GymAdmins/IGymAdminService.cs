using FitHub.Application.Common;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.GymAdmins;

public interface IGymAdminService
{
    Task<PagedResult<GymAdmin>> GetAll(PagedQuery query, CancellationToken ct);
}
