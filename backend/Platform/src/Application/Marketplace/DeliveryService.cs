using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public class DeliveryService : IDeliveryService
{
    private readonly IDeliveryRepository deliveryRepository;

    public DeliveryService(IDeliveryRepository deliveryRepository)
    {
        this.deliveryRepository = deliveryRepository;
    }

    public Task<Delivery?> GetDeliveryAsync(DeliveryId deliveryId, CancellationToken ct)
    {
        return deliveryRepository.GetByIdAsync(deliveryId, ct);
    }

    public Task<Delivery?> GetDeliveryByOrderAsync(MarketplaceOrderId orderId, CancellationToken ct)
    {
        return deliveryRepository.GetByOrderIdAsync(orderId, ct);
    }

    public Task<Delivery?> GetDeliveryByOrderForUserAsync(MarketplaceOrderId orderId, IdentityUserId userId, CancellationToken ct)
    {
        return deliveryRepository.GetByOrderIdForUserAsync(orderId, userId, ct);
    }

    public Task<PagedResult<Delivery>> GetDeliveriesAsync(PagedQuery paged, CancellationToken ct)
    {
        return deliveryRepository.GetAllAsync(paged, ct);
    }
}
