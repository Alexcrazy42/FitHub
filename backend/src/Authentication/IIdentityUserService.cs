using FitHub.Common.AspNetCore.Accounting;

namespace FitHub.Authentication;

/// <summary>
/// Сервис работы с пользователями
/// </summary>
public interface IIdentityUserService
{
    /// <summary>
    /// Получить пользователя по его имени
    /// </summary>
    Task<IdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Получить пользователя по его id
    /// </summary>
    Task<IdentityUser?> GetOrDefaultAsync(IdentityUserId id, CancellationToken cancellationToken);

    /// <summary>
    /// Получить пользователя по id
    /// </summary>
    Task<IdentityUser> GetAsync(IdentityUserId id, CancellationToken cancellationToken);

    /// <summary>
    /// Валидна ли сессия
    /// </summary>
    Task<bool> IsSessionValid(IdentityUserId userId, string sessionId, CancellationToken ct = default);
}
