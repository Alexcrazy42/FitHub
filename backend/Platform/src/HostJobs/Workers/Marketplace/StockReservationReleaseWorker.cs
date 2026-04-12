using FitHub.Clients.Marketplace;

namespace FitHub.HostJobs.Workers.Marketplace;

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
                var marketplaceJobsClient = scope.ServiceProvider.GetRequiredService<IMarketplaceJobsClient>();
                var result = await marketplaceJobsClient.ReleaseExpiredReservationsAsync(stoppingToken);

                if (result.ReleasedCount > 0)
                {
                    logger.LogInformation("{WorkerName} released expired marketplace reservations: {ReleasedCount}", GetType().Name, result.ReleasedCount);
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
