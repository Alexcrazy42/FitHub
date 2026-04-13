using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public interface IDeliveryAssignmentService
{
    Task<int> AutoAssignPendingAsync(int batchSize, TimeSpan acceptanceTimeout, CancellationToken ct);

    Task<int> ReleaseExpiredAssignmentsAsync(DateTimeOffset now, int batchSize, CancellationToken ct);

    Task<int> EnsureCouriersAsync(IReadOnlyCollection<string> courierNames, CancellationToken ct);

    Task<Delivery> AcceptAssignmentAsync(DeliveryId deliveryId, CourierId courierId, CancellationToken ct);

    Task<Delivery> RejectAssignmentAsync(DeliveryId deliveryId, CourierId courierId, string? reason, CancellationToken ct);
}
