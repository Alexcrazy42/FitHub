using FitHub.Clients.Marketplace;
using FitHub.Contracts.V1.Marketplace;
using FitHub.Queue.Contracts.Marketplace;
using FitHub.RabbitMQ.Consumers;
using Microsoft.Extensions.Options;

namespace FitHub.Simulation.CourierSimulationJobs.Consumers;

[Consumer("courier-simulation.delivery.assigned.queue", "delivery.courier.assigned")]
public sealed class CourierDeliveryAssignedConsumer : IRabbitMqConsumerHandler<CourierDeliveryAssignedMessage>
{
    private readonly IMarketplaceJobsClient marketplaceJobsClient;
    private readonly IOptions<CourierSimulationOptions> options;
    private readonly ILogger<CourierDeliveryAssignedConsumer> logger;

    public CourierDeliveryAssignedConsumer(
        IMarketplaceJobsClient marketplaceJobsClient,
        IOptions<CourierSimulationOptions> options,
        ILogger<CourierDeliveryAssignedConsumer> logger)
    {
        this.marketplaceJobsClient = marketplaceJobsClient;
        this.options = options;
        this.logger = logger;
    }

    public async Task HandleAsync(CourierDeliveryAssignedMessage message, CancellationToken ct)
    {
        var delay = TimeSpan.FromSeconds(Math.Max(0, options.Value.DecisionDelaySeconds));

        if (delay > TimeSpan.Zero)
        {
            await Task.Delay(delay, ct);
        }

        var decision = options.Value.AssignmentDecision.Equals("reject", StringComparison.OrdinalIgnoreCase)
            ? "reject"
            : "accept";

        var response = await marketplaceJobsClient.ApplyCourierAssignmentDecisionAsync(
            new CourierAssignmentDecisionRequest(
                message.DeliveryId,
                message.CourierId,
                decision,
                decision == "reject" ? "Courier simulation rejected assignment." : null),
            ct);

        logger.LogInformation(
            "Courier simulation applied {Decision} decision for delivery {DeliveryId}. New status: {Status}",
            decision,
            response.DeliveryId,
            response.Status);
    }
}
