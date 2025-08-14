using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.AspNetCore.Problems;
using FitHub.Common.AspNetCore.Tokens;
using FitHub.Common.Entities;
using FitHub.Common.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using IAuthenticationService = FitHub.Common.AspNetCore.Auth.IAuthenticationService;

namespace FitHub.Common.AspNetCore;

public static class ServiceRegistry
{
    public static void AddExceptionAsProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionLoggingHandler>()
            .AddExceptionHandler<GlobalExceptionHandler>();

        services.AddSingleton<IExceptionToProblemRegistry, CommonExceptionToProblemRegistry>();
    }

    /// <summary>
    /// Включить обработку исключений как <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/>
    /// </summary>
    public static IApplicationBuilder UseExceptionAsProblemDetails(this IApplicationBuilder app, bool useExceptionPage)
    {
        if (useExceptionPage)
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseExceptionHandler();

        return app;
    }

    /// <summary>
    /// Добавить аутентификацию и авторизацию, основанную на Jwt-токенах
    /// </summary>
    public static void AddJwtAuthenticationAndAuthorization<TAuthOptions>(
        this IServiceCollection services,
        IAuthOptions authOptions,
        Action<JwtBearerOptions>? configureOptions = null)
        where TAuthOptions : class, IAuthOptions
    {
        services.AddCurrentIdentityUserAccessor();

        services.AddBindedOptionsAs<TAuthOptions, IAuthOptions>();
        services
            .AddMvcCore()
            .AddApiExplorer()
            .AddApplicationPart(typeof(ServiceRegistry).Assembly)
            .AddControllersAsServices();

        services.AddAuthorization();

        services.AddTransient<ITokenService, TokenService>();

        services
            .AddAuthentication(AuthenticationScheme.Bearer)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = TokenDefaults.CreateTokenValidationParameters(
                    authOptions.RequiredIssuer,
                    signingKey: SigningKey.GetSymmetricSecurityKey(authOptions.RequiredSecretKey));

                // Add this to read the token from the cookie
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[IAuthOptions.CookieName];
                        return Task.CompletedTask;
                    }
                };

                configureOptions?.Invoke(options);
            });
    }

    public static void UseAuthentificationAndAuthorization(
        this IApplicationBuilder app,
        IAuthOptions authOptions)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapPost(authOptions.LoginRoute,
                async Task<IResult> (
                    IAuthenticationService authenticationService,
                    ITokenService tokenService,
                    HttpContext context,
                    LoginRequest request, CancellationToken cancellationToken) =>
                {
                    ValidationException.ThrowIfNull(request.Username);
                    ValidationException.ThrowIfNull(request.Password);

                    var identityUser = await authenticationService.LoginAsync(request.Username, request.Password, cancellationToken);

                    if (identityUser is null)
                    {
                        return Results.Unauthorized();
                    }

                    var expiresAt = DateTimeOffset.UtcNow
                        .Add(authOptions.RequiredCookieExpiration)
                        .DateTime;

                    var claims = ITokenService.CreateCommonClaims(identityUser.Id.ToString());

                    var tokenString = tokenService.Create(claims);

                    context.Response.Cookies.Append(IAuthOptions.CookieName, tokenString, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = expiresAt,
                        Path = "/"
                    });
                    var response = new LoginResponse
                    {
                        Login = identityUser.Email,
                        Name = identityUser.Name,
                        UserId = identityUser.Id.Value.ToString(),
                        LoginExpirationAt = expiresAt
                    };
                    return Results.Ok(response);
                })
                .WithTags("Auth");
        });
    }

    private static void AddCurrentIdentityUserAccessor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentIdentityUserIdProvider, HttpCurrentIdentityUserIdProvider>();

        services.AddScoped<ICurrentIdentityUserIdAccessor, CurrentIdentityUserIdAccessor>();

        services.AddScoped<ICurrentIdentityUserAccessor, CurrentIdentityUserAccessor>();
    }
}
