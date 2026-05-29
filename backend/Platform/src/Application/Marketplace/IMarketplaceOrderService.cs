using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public interface IMarketplaceOrderService
{
    Task<MarketplaceOrder?> GetOrderAsync(MarketplaceOrderId orderId, CancellationToken ct);

    Task<PagedResult<MarketplaceOrder>> GetOrdersAsync(IdentityUserId userId, PagedQuery paged, CancellationToken ct);
}
