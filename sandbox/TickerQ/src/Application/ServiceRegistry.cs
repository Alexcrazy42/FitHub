using Microsoft.Extensions.DependencyInjection;
using TickerQ.Jobs.Application.Notifications;

namespace TickerQ.Jobs.Application;

public static class ServiceRegistry
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddNotifications();
    }

    public static void AddNotifications(this IServiceCollection services)
    {
        services.AddTransient<IEmailNotificationService, EmailNotificationService>();
    }
}
