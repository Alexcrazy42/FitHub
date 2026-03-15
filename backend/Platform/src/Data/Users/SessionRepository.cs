using FitHub.Application.Users;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Users;

namespace FitHub.Data.Users;

public class SessionRepository : DefaultPendingRepository<Session, SessionId, DataContext>, ISessionRepository
{
    public SessionRepository(DataContext context) : base(context)
    {
    }
}
