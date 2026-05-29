using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public class MarketplaceOrderService : IMarketplaceOrderService
{
    private readonly IMarketplaceOrderRepository orderRepository;

    public MarketplaceOrderService(IMarketplaceOrderRepository orderRepository)
    {
        this.orderRepository = orderRepository;
    }

    public Task<MarketplaceOrder?> GetOrderAsync(MarketplaceOrderId orderId, CancellationToken ct)
    {
        return orderRepository.GetByIdAsync(orderId, ct);
    }

    public Task<PagedResult<MarketplaceOrder>> GetOrdersAsync(IdentityUserId userId, PagedQuery paged, CancellationToken ct)
    {
        return orderRepository.GetByUserIdAsync(userId, paged, ct);
    }
}
