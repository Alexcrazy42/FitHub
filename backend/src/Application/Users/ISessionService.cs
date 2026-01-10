using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Domain.Users;

namespace FitHub.Application.Users;

public interface ISessionService
{
    Task<Session> Create(Session session, CancellationToken ct = default);

    Task<bool> IsSessionValid(SessionId id, IdentityUserId userId, CancellationToken ct = default);

    Task<Session> Get(SessionId id, CancellationToken ct = default);
}
