using FitHub.Clients.Marketplace;
using FitHub.Contracts.V1.Marketplace;
using Microsoft.Extensions.Options;

namespace FitHub.Simulation.CourierSimulationJobs.Workers;

public class CourierSimulationBootstrapWorker : BackgroundService
{
    private readonly IServiceProvider provider;
    private readonly IOptions<CourierSimulationOptions> options;
    private readonly ILogger<CourierSimulationBootstrapWorker> logger;

    public CourierSimulationBootstrapWorker(
        IServiceProvider provider,
        IOptions<CourierSimulationOptions> options,
        ILogger<CourierSimulationBootstrapWorker> logger)
    {
        this.provider = provider;
        this.options = options;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = provider.CreateScope();
            var marketplaceJobsClient = scope.ServiceProvider.GetRequiredService<IMarketplaceJobsClient>();
            var response = await marketplaceJobsClient.EnsureCouriersAsync(
                new EnsureCouriersRequest(options.Value.CourierNames),
                stoppingToken);

            logger.LogInformation(
                "Courier simulation ensured debug couriers. Created: {CreatedCount}",
                response.CreatedCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }

    }
}
