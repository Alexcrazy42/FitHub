using FitHub.BankManager.Clients.Payment;

namespace FitHub.Jobs.Workers;

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
                var paymentClient = scope.ServiceProvider.GetRequiredService<IBankManagerPaymentClient>();
                var result = await paymentClient.PublishOutboxAsync(stoppingToken);

                if (result.PublishedCount > 0 || result.FailedCount > 0)
                {
                    logger.LogInformation(
                        "Published BankManager outbox messages: {PublishedCount}, failed: {FailedCount}",
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
                logger.LogError(ex, "Failed to request BankManager outbox publishing.");
            }

            await Task.Delay(Delay, stoppingToken);
        }
    }
}
