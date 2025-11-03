using FitHub.Application.EmailNotifications;
using FitHub.Common.EntityFramework;
using FitHub.Common.Extensions.Configuration;
using FitHub.Data;
using FitHub.Data.EmailNotifications;
using FitHub.HostJobs.Workers.EmailNotifications;

namespace FitHub.HostJobs;

public static class ServiceRegistry
{
    public static void AddServicesForBackground(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<EmailNotificationSenderWorker>();
        services.AddScoped<IEmailNotificationService, EmailNotificationService>();
        services.AddScoped<IEmailNotificationRepository, EmailNotificationRepository>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddBindedOptions<ConnectionOptions>();

        var databaseOptions = configuration.GetRequiredOptions<ConnectionOptions>();
        services.AddDataContext<DataContext>(databaseOptions);
        services.AddUnitOfWork<DataContext>(databaseOptions);
    }
}
