using FitHub.Application.Common;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.CmsAdmins;

public interface ICmsAdminService
{
    Task<PagedResult<User>> GetCmsAdmins(PagedQuery query, CancellationToken ct);
}
