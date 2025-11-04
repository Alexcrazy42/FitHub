namespace FitHub.Common.AspNetCore.Accounting;

/// <summary>
/// Сервис, для получения текущего пользователя
/// </summary>
public interface ICurrentIdentityUserAccessor
{
    /// <summary>
    /// Получить текущего юзера
    /// </summary>
    public Task<IdentityUser> GetCurrentIdentityUser(CancellationToken cancellationToken);
}
