using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public interface IDeliveryService
{
    Task<Delivery?> GetDeliveryAsync(DeliveryId deliveryId, CancellationToken ct);

    Task<Delivery?> GetDeliveryByOrderAsync(MarketplaceOrderId orderId, CancellationToken ct);

    Task<Delivery?> GetDeliveryByOrderForUserAsync(MarketplaceOrderId orderId, IdentityUserId userId, CancellationToken ct);

    Task<PagedResult<Delivery>> GetDeliveriesAsync(PagedQuery paged, CancellationToken ct);
}
