using FitHub.Domain.Users;

namespace FitHub.Application.Users;

public interface ISessionService
{
    Task<Session> Create(Session session, CancellationToken ct = default);
}
