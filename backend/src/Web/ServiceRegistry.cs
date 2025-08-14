using FitHub.Common.AspNetCore;
using FitHub.Common.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Web;

public static class ServiceRegistry
{
    public static void AddWeb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBindedOptions<AuthOptions>();
        var authOptions = configuration.GetRequiredOptions<AuthOptions>();
        services.AddJwtAuthenticationAndAuthorization<AuthOptions>(authOptions);

        services
            .AddMvcCore()
            .AddApiExplorer()
            .AddApplicationPart(typeof(Web.ServiceRegistry).Assembly)
            .AddControllersAsServices();

        services.AddExceptionAsProblemDetails();
        services.AddSignalR();
    }

    public static void UseWeb(this IApplicationBuilder app, IConfiguration configuration)
    {
        var authOptions = configuration.GetRequiredOptions<AuthOptions>();

        app.UseAuthentificationAndAuthorization(authOptions);

        app.UseEndpoints(configure =>
        {
            configure.MapControllers().RequireAuthorization();
            // configure.MapHub<NotificationHub>("/notificationHub");
        });

    }
}
