using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.Visitors;

public interface IVisitorRepository : IPendingRepository<Visitor, VisitorId>
{
    Task<PagedResult<Visitor>> GetAll(PagedQuery query, CancellationToken ct);
}
