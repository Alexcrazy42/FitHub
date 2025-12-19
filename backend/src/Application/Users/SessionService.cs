using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Users;

namespace FitHub.Application.Users;

public class SessionService : ISessionService
{
    private readonly ISessionRepository sessionRepository;
    private readonly IUnitOfWork unitOfWork;

    public SessionService(ISessionRepository sessionRepository, IUnitOfWork unitOfWork)
    {
        this.sessionRepository = sessionRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Session> Create(Session session, CancellationToken ct = default)
    {
        await sessionRepository.PendingAddAsync(session, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return session;
    }

    public async Task<bool> IsSessionValid(SessionId id, IdentityUserId userId, CancellationToken ct = default)
    {
        try
        {
            var session = await Get(id, ct);
            return session.UserId == userId && session.ExpiresOn > DateTimeOffset.UtcNow && session.IsActive;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<Session> Get(SessionId id, CancellationToken ct = default)
    {
        var session = await sessionRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(session, "Сессия не найдена!");
        return session;
    }
}
