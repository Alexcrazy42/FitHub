using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Contracts.V1.Users.Visitors;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.Visitors;

public interface IVisitorService
{
    Task<PagedResult<Visitor>> GetAll(PagedQuery query, VisitorSearchRequest? request, CancellationToken ct);

    Task<IReadOnlyList<Visitor>> GetVisitorsAsync(IReadOnlyList<VisitorId> ids, CancellationToken ct);

    Task SetStatus(VisitorId id, bool status, CancellationToken ct);

    Task<Visitor> GetByUserIdAsync(IdentityUserId userId, CancellationToken ct);
}
