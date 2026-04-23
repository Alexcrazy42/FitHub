using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Domain.Marketplace;
using FitHub.Domain.Marketplace.Deliveries;

namespace FitHub.Application.Marketplace.Deliveries;

public interface IDeliveryService
{
    Task<Delivery?> GetDeliveryAsync(DeliveryId deliveryId, CancellationToken ct);

    Task<Delivery?> GetDeliveryByOrderAsync(MarketplaceOrderId orderId, CancellationToken ct);

    Task<Delivery?> GetDeliveryByOrderForUserAsync(MarketplaceOrderId orderId, IdentityUserId userId, CancellationToken ct);

    Task<PagedResult<Delivery>> GetDeliveriesAsync(PagedQuery paged, CancellationToken ct);
}
