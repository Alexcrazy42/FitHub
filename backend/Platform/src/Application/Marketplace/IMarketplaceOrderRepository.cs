using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public interface IMarketplaceOrderRepository : IPendingRepository<MarketplaceOrder, MarketplaceOrderId>
{
    Task<MarketplaceOrder?> GetByIdAsync(MarketplaceOrderId orderId, CancellationToken ct);

    Task<MarketplaceOrder?> GetByReservationIdAsync(StockReservationId reservationId, CancellationToken ct);

    Task<MarketplaceOrder?> GetByPaymentIdAsync(MarketplacePaymentId paymentId, CancellationToken ct);

    Task<PagedResult<MarketplaceOrder>> GetByUserIdAsync(IdentityUserId userId, PagedQuery paged, CancellationToken ct);
}
