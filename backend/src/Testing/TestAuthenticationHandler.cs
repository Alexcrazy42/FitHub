using System.Security.Claims;
using System.Text.Encodings.Web;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using AuthenticationScheme = FitHub.Common.Identity.Client.AuthenticationScheme;

namespace FitHub.Common.Testing;

public class TestAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
}

internal sealed class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationHandlerOptions>
{
    private readonly ITokenService tokenService;

    public TestAuthenticationHandler(
        IOptionsMonitor<TestAuthenticationHandlerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ITokenService tokenService) : base(options, logger, encoder, clock)
    {
        this.tokenService = tokenService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = Request.Headers[HeaderNames.Authorization];

        // Токен для нашего сервиса должен быть только один
        if (authorization.Count != 1)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var token = authorization.Single();

        // Работаем только с Bearer токеном
        if (token == null || !token.StartsWith(AuthenticationScheme.Bearer, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        // Удаляем префикс токена
        token = token.Substring(AuthenticationScheme.Bearer.Length).Trim();

        try
        {
            var principal = tokenService.Validate(token);

            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception exception)
        {
            return Task.FromResult(AuthenticateResult.Fail(exception));
        }
    }
}
