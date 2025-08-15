using FitHub.Domain.Users;

namespace FitHub.Application.Users;

public interface IUserService
{
    /// <summary>
    /// Зарегистрировать администратора зала
    /// </summary>
    Task<User> RegisterAdminAsync(CancellationToken ct = default);
}
