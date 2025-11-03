using FitHub.Common.AspNetCore.Auth;
using FitHub.Contracts.V1.Auth;
using FitHub.Domain.Users;

namespace FitHub.Application.Users;

public interface IUserService
{
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
    Task InitResetPasswordAsync(CancellationToken ct = default);

    Task<bool> CheckResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);

    /// <summary>
    /// Сменить пароль
    /// </summary>
    Task<LoginResponse> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);
}
