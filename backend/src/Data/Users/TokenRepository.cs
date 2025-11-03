using FitHub.Application.Users;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Users;

namespace FitHub.Data.Users;

public class TokenRepository :
    DefaultPendingRepository<Token, TokenId, DataContext>,
    ITokenRepository
{
    public TokenRepository(DataContext context) : base(context)
    {
    }
}
