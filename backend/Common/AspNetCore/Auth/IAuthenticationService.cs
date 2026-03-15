
using FitHub.Common.AspNetCore.Accounting;

namespace FitHub.Common.AspNetCore.Auth;

/// <summary>
/// Сервис аутентификации
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Попытаться залогинить пользователя по паролю
    /// </summary>
    Task<LoginResponse> LoginAsync(string login, string password, CancellationToken cancellationToken);
}
