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
}
