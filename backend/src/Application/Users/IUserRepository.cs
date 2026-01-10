using FitHub.Application.Common;
using FitHub.Application.Users.Commands;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Users;

namespace FitHub.Application.Users;

public interface IUserRepository : IPendingRepository<User, IdentityUserId>
{
    Task<PagedResult<User>> GetCmsAdminsAsync(PagedQuery query, CancellationToken ct);

    Task<IReadOnlyList<User>> GetUsersAsync(List<IdentityUserId> userIds, CancellationToken ct);

    Task<PagedResult<User>> GetPagedUsersAsync(GetUserQuery query, PagedQuery pagedQuery, CancellationToken ct);
}
