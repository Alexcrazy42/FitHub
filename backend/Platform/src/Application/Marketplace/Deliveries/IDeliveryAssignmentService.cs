using FitHub.Domain.Marketplace;
using FitHub.Domain.Marketplace.Deliveries;

namespace FitHub.Application.Marketplace.Deliveries;

public interface IDeliveryAssignmentService
{
    Task<int> AutoAssignPendingAsync(int batchSize, TimeSpan acceptanceTimeout, CancellationToken ct);

    Task<int> ReleaseExpiredAssignmentsAsync(DateTimeOffset now, int batchSize, CancellationToken ct);

    Task<int> EnsureCouriersAsync(IReadOnlyCollection<string> courierNames, CancellationToken ct);

    Task<Delivery> AcceptAssignmentAsync(DeliveryId deliveryId, CourierId courierId, CancellationToken ct);

    Task<Delivery> RejectAssignmentAsync(DeliveryId deliveryId, CourierId courierId, string? reason, CancellationToken ct);
}
