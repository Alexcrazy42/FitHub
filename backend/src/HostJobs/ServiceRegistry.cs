using FitHub.Application.EmailNotifications;
using FitHub.Application.Users;
using FitHub.Clients;
using FitHub.Common.EntityFramework;
using FitHub.Common.Extensions.Configuration;
using FitHub.Data;
using FitHub.Data.EmailNotifications;
using FitHub.Data.Users;
using FitHub.HostJobs.Workers.EmailNotifications;
using FitHub.HostJobs.Workers.Tokens;
using FitHub.HostJobs.Workers.Videos;
using FitHub.RabbitMQ;
using FitHub.RabbitMQ.Configuration;
using FitHub.RabbitMQ.Contracts.Videos;

namespace FitHub.HostJobs;

public static class ServiceRegistry
{
    public static void AddServicesForBackground(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<EmailNotificationSenderWorker>();
        services.AddHostedService<TokenCleanerWorker>();

        services.AddScoped<IEmailNotificationService, EmailNotificationService>();
        services.AddScoped<IEmailNotificationRepository, EmailNotificationRepository>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<ITokenRepository, TokenRepository>();

        services.AddBindedOptions<ConnectionOptions>();
        var databaseOptions = configuration.GetRequiredOptions<ConnectionOptions>();
        services.AddDataContext<DataContext>(databaseOptions);
        services.AddUnitOfWork<DataContext>(databaseOptions);

        services.AddRabbitMq<RabbitMqClusterOptions>();
        services.AddConsumerAsBackgroundService<VideoEncodingMessage, VideoEncodingConsumer, RabbitMqClusterOptions>();

        services.AddFitHubClients();
    }
}
