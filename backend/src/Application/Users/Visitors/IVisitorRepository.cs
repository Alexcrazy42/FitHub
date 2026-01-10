using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Users.Visitors;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.Visitors;

public interface IVisitorRepository : IPendingRepository<Visitor, VisitorId>
{
    Task<PagedResult<Visitor>> GetAll(PagedQuery query, VisitorSearchRequest? request, CancellationToken ct);

    Task<IReadOnlyList<Visitor>> GetAsync(IReadOnlyList<VisitorId> ids, CancellationToken ct);

    Task<Visitor> GetAsync(VisitorId id, CancellationToken ct);

    Task<Visitor> GetByUserIdAsync(IdentityUserId userId, CancellationToken ct);
}
