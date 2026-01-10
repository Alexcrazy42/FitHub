namespace FitHub.Authentication;

/// <summary>
/// Поставщик идентификатора текущего пользователя
/// </summary>
public interface ICurrentIdentityUserIdProvider
{
    /// <summary>
    /// Получить текущего пользователя
    /// </summary>
    public IdentityUserId? GetCurrentUserIdOrDefault();

    /// <summary>
    /// Получить Id сессии
    /// </summary>
    public string? GetSessionId();
}
