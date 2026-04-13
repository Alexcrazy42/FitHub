using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public interface IDeliveryRepository : IPendingRepository<Delivery, DeliveryId>
{
    Task<Delivery?> GetByIdAsync(DeliveryId deliveryId, CancellationToken ct);

    Task<Delivery?> GetByOrderIdAsync(MarketplaceOrderId orderId, CancellationToken ct);

    Task<Delivery?> GetByOrderIdForUserAsync(MarketplaceOrderId orderId, IdentityUserId userId, CancellationToken ct);

    Task<PagedResult<Delivery>> GetAllAsync(PagedQuery paged, CancellationToken ct);

    Task<IReadOnlyList<Delivery>> GetPendingForAssignmentAsync(int take, CancellationToken ct);

    Task<IReadOnlyList<Delivery>> GetExpiredCourierAssignmentsAsync(DateTimeOffset now, int take, CancellationToken ct);
}
