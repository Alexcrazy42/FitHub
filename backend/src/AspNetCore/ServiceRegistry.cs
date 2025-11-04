using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.AspNetCore.Problems;
using FitHub.Common.AspNetCore.Tokens;
using FitHub.Common.Entities;
using FitHub.Common.Extensions.Configuration;
using FitHub.Common.Utilities.System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.CmsAdminOnly, policy =>
                policy.RequireAssertion(context => context.User.HasUserType(IdentityUserType.CmsAdmin)));

            options.AddPolicy(AuthorizationPolicies.TrainerOnly, policy =>
                policy.RequireAssertion(context => context.User.HasUserType(IdentityUserType.Trainer)));

            options.AddPolicy(AuthorizationPolicies.GymAdminOnly, policy =>
                policy.RequireAssertion(context => context.User.HasUserType(IdentityUserType.GymAdmin)));

            options.AddPolicy(AuthorizationPolicies.GymVisitorOnly, policy =>
                policy.RequireAssertion(context => context.User.HasUserType(IdentityUserType.GymVisitor)));
        });

        services.AddTransient<ITokenService, TokenService>();

        services
            .AddAuthentication(AuthenticationScheme.Bearer)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = TokenDefaults.CreateTokenValidationParameters(
                    authOptions.RequiredIssuer,
                    signingKey: SigningKey.GetSymmetricSecurityKey(authOptions.RequiredSecretKey));

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[IAuthOptions.CookieName];
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var identityUserService = context.HttpContext.RequestServices.GetRequiredService<IIdentityUserService>();

                        var sessionId = context.Principal?.GetSessionId();

                        if (sessionId is null)
                        {
                            context.Fail("SessionId is null");
                            return;
                        }

                        var userId = context.Principal?.GetUserId();

                        if (userId is null)
                        {
                            context.Fail("UserId is null");
                            return;
                        }

                        var isValidSession = await identityUserService.IsSessionValid(IdentityUserId.Parse(userId), sessionId.Required());
                        if (!isValidSession)
                        {
                            context.Fail("Session is inactive or revoked");
                        }
                    }
                };

                configureOptions?.Invoke(options);
            });
    }

    public static void UseAuthenticationAndAuthorization(
        this IApplicationBuilder app,
        IAuthOptions authOptions)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapPost(authOptions.LoginRoute,
                async Task<IResult> (
                    [FromServices] IAuthenticationService authenticationService,
                    [FromServices] ITokenService tokenService,
                    [FromBody] LoginRequest request,
                    CancellationToken cancellationToken) =>
                {
                    ValidationException.ThrowIfNull(request.Username);
                    ValidationException.ThrowIfNull(request.Password);

                    var loginResponse = await authenticationService.LoginAsync(request.Username, request.Password, cancellationToken);

                    if (!loginResponse.LoginFlowDone.Required())
                    {
                        return Results.Unauthorized();
                    }

                    return Results.Ok(loginResponse);
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
