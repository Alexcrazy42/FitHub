namespace FitHub.Common.AspNetCore.Accounting;

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
}
