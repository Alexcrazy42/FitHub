using FitHub.Clients.Marketplace;

namespace FitHub.HostJobs.Workers.Marketplace;

public class DeliveryAutoAssignmentWorker : BackgroundService
{
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(10);

    private readonly IServiceProvider provider;
    private readonly ILogger<DeliveryAutoAssignmentWorker> logger;

    public DeliveryAutoAssignmentWorker(IServiceProvider provider, ILogger<DeliveryAutoAssignmentWorker> logger)
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
                var result = await marketplaceJobsClient.AutoAssignDeliveriesAsync(stoppingToken);

                if (result.AssignedCount > 0 || result.ReleasedExpiredAssignmentsCount > 0)
                {
                    logger.LogInformation(
                        "{WorkerName} assigned deliveries: {AssignedCount}, released expired assignments: {ReleasedCount}",
                        GetType().Name,
                        result.AssignedCount,
                        result.ReleasedExpiredAssignmentsCount);
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
