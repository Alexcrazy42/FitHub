using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Contracts.V1.Auth;
using FitHub.Contracts.V1.Users.GymAdmins;
using FitHub.Contracts.V1.Users.Trainers;
using FitHub.Domain.Users;

namespace FitHub.Application.Users;

public interface IUserService
{
    /// <summary>
    /// Получить пользователя
    /// </summary>
    Task<User> GetUserAsync(IdentityUserId userId, CancellationToken ct);

    Task<IReadOnlyList<User>> GetUsersAsync(List<IdentityUserId> userIds, CancellationToken ct);

    /// <summary>
    /// Начать регистрацию
    /// </summary>
    Task<User> StartRegister(StartRegisterRequest request, CancellationToken ct);

    /// <summary>
    /// Зарегистрировать Cms-администратора
    /// </summary>
    Task<User> RegisterCmsAdminAsync(CreateCmsAdminRequest request, CancellationToken ct = default);

    /// <summary>
    /// Зарегистрировать администратора зала
    /// </summary>
    Task<User> RegisterGymAdminAsync(CreateGymAdminRequest request, CancellationToken ct = default);

    /// <summary>
    /// Зарегистрировать тренера
    /// </summary>
    Task<User> RegisterTrainerAsync(CreateTrainerRequest request, CancellationToken ct = default);

    Task<bool> CheckConfirmEmail(ConfirmEmailRequest request, CancellationToken ct = default);

    /// <summary>
    /// Подтвердить email
    /// </summary>
    Task<LoginResponse> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken ct = default);

    /// <summary>
    /// Задать пароль
    /// </summary>
    Task<LoginResponse> SetPasswordAsync(SetPasswordRequest request, CancellationToken ct = default);

    /// <summary>
    /// Стартовать изменение пароля
    /// </summary>
    Task InitResetPasswordAsync(string email, CancellationToken ct = default);

    Task<bool> CheckResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);

    /// <summary>
    /// Сменить пароль
    /// </summary>
    Task<LoginResponse> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);

    /// <summary>
    /// Разлогиниться
    /// </summary>
    Task Logout(IdentityUserId userId, SessionId sessionId, CancellationToken ct = default);
}
