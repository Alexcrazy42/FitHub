using FitHub.Common.Entities.Storage;
using FitHub.Domain.Users;

namespace FitHub.Application.Users;

public interface ITokenRepository : IPendingRepository<Token, TokenId>
{
    Task ClearInactiveTokens(CancellationToken ct);
}
