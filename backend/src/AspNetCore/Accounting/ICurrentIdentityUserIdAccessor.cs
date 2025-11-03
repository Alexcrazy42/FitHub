namespace FitHub.Common.AspNetCore.Accounting;

/// <summary>
/// Сервис для получения идентификатора текущего пользователя
/// </summary>
public interface ICurrentIdentityUserIdAccessor
{
    /// <summary>
    /// Возвращает идентификатор текущего пользователя
    /// </summary>
    public IdentityUserId GetCurrentUserId();
}
