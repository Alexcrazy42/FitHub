using FitHub.Common.Entities.Storage;
using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public interface IMarketplacePaymentRepository : IPendingRepository<MarketplacePayment, MarketplacePaymentId>
{
    Task<MarketplacePayment?> GetByReservationIdAsync(StockReservationId reservationId, CancellationToken ct);
}
