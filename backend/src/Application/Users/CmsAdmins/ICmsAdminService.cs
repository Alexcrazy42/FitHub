using FitHub.Application.Common;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.CmsAdmins;

public interface ICmsAdminService
{
    Task<PagedResult<User>> GetCmsAdmins(PagedQuery query, CancellationToken ct);

    Task SetStatus(IdentityUserId id, bool status, CancellationToken ct);
}
