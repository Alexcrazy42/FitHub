using FitHub.Application.Users;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Users;

public class TokenRepository :
    DefaultPendingRepository<Token, TokenId, DataContext>,
    ITokenRepository
{
    public TokenRepository(DataContext context) : base(context)
    {
    }

    public Task ClearInactiveTokens(CancellationToken ct)
    {
        var query = ReadRaw()
            .Where(x => x.AppliedAt != null || x.ExpiresOn < DateTime.UtcNow);

        return query.ExecuteDeleteAsync(ct);
    }
}
