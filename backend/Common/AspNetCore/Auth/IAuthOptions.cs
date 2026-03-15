using FitHub.Common.Extensions.Configuration;
using FitHub.Common.Utilities.System;

namespace FitHub.Common.AspNetCore.Auth;

/// <summary>
/// Конфигурация аутентификации/авторизации
/// </summary>
public interface IAuthOptions : IHaveConfigSection
{
    static string IHaveConfigSection.SectionName
        => throw new NotImplementedException("Переопределите в дочерних классах");

    /// <summary>
    /// Издатель токена
    /// </summary>
    string? Issuer { get; }

    string RequiredIssuer => Issuer.Required();

    /// <summary>
    /// Секретный ключ для шифрования
    /// </summary>
    string? SecretKey { get; }

    string RequiredSecretKey => SecretKey.Required();

    /// <summary>
    /// Время, за которое кука протухнет
    /// </summary>
    TimeSpan? CookieExpiration { get; }


    TimeSpan RequiredCookieExpiration => CookieExpiration.Required();

    /// <summary>
    /// Относительный путь для аутентификации
    /// </summary>
    string LoginRoute { get; }

    /// <summary>
    /// Имя присваемое куки аутентификации
    /// </summary>
    // TODO: Сделать общую аутентификацию/авторизацию (айдентити сервер),
    // в текущей конфигурации куки разных сервисов будут друг друга перетирать
    static string CookieName => "FitHub.Identity.Cookie";
}
