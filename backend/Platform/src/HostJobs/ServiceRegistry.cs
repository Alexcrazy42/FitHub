using FitHub.BankManager.Rabbit.Contracts.Payments;
using FitHub.Clients;
using FitHub.HostJobs.Consumers.Marketplace;
using FitHub.HostJobs.Consumers.Videos;
using FitHub.HostJobs.Workers.Marketplace;
using FitHub.HostJobs.Workers.Outbox;
using FitHub.Queue.Contracts.Videos;
using FitHub.RabbitMQ;
using FitHub.RabbitMQ.Configuration;

namespace FitHub.HostJobs;

public static class ServiceRegistry
{
    public static void AddServicesForBackground(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRabbitMq<RabbitMqClusterOptions>();
        services.AddConsumerAsBackgroundService<VideoEncodingMessage, VideoEncodingConsumer, RabbitMqClusterOptions>();
        services.AddConsumerAsBackgroundService<PaymentStatusChangedMessage, PaymentStatusChangedConsumer, RabbitMqClusterOptions>();
        services.AddHostedService<StockReservationReleaseWorker>();
        services.AddHostedService<DeliveryAutoAssignmentWorker>();
        services.AddHostedService<RabbitOutboxPublisherWorker>();

        services.AddFitHubClients();
    }
}
