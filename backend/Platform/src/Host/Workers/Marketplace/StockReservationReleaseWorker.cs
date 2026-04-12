using FitHub.Application.Marketplace;

namespace FitHub.Host.Workers.Marketplace;

public class StockReservationReleaseWorker : BackgroundService
{
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(30);

    private readonly IServiceProvider provider;
    private readonly ILogger<StockReservationReleaseWorker> logger;

    public StockReservationReleaseWorker(IServiceProvider provider, ILogger<StockReservationReleaseWorker> logger)
    {
        this.provider = provider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{WorkerName} starting", GetType().Name);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = provider.CreateScope();
                var checkoutService = scope.ServiceProvider.GetRequiredService<IMarketplaceCheckoutService>();
                var releasedCount = await checkoutService.ReleaseExpiredReservationsAsync(DateTimeOffset.UtcNow, stoppingToken);

                if (releasedCount > 0)
                {
                    logger.LogInformation("{WorkerName} released expired marketplace reservations: {ReleasedCount}", GetType().Name, releasedCount);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error throw from worker {WorkerName}", GetType().Name);
            }

            await Task.Delay(Delay, stoppingToken);
        }

        logger.LogInformation("{WorkerName} stopping", GetType().Name);
    }
}
