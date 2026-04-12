using FitHub.Clients.Marketplace;

namespace FitHub.HostJobs.Workers.Outbox;

public sealed class RabbitOutboxPublisherWorker : BackgroundService
{
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(2);

    private readonly IServiceProvider provider;
    private readonly ILogger<RabbitOutboxPublisherWorker> logger;

    public RabbitOutboxPublisherWorker(IServiceProvider provider, ILogger<RabbitOutboxPublisherWorker> logger)
    {
        this.provider = provider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = provider.CreateScope();
                var marketplaceJobsClient = scope.ServiceProvider.GetRequiredService<IMarketplaceJobsClient>();
                var result = await marketplaceJobsClient.PublishOutboxAsync(stoppingToken);

                if (result.PublishedCount > 0 || result.FailedCount > 0)
                {
                    logger.LogInformation(
                        "Published Platform outbox messages: {PublishedCount}, failed: {FailedCount}",
                        result.PublishedCount,
                        result.FailedCount);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to request Platform outbox publishing.");
            }

            await Task.Delay(Delay, stoppingToken);
        }
    }
}
