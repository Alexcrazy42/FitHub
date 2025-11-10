using FitHub.Application.Common;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.Visitors;

public interface IVisitorService
{
    Task<PagedResult<Visitor>> GetAll(PagedQuery query, CancellationToken ct);
}
