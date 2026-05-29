using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public interface IMarketplacePaymentService
{
    Task<MarketplacePaymentResult> CreatePaymentIntentAsync(StockReservationId reservationId, CancellationToken ct);

    Task ApplyBankPaymentStatusAsync(
        StockReservationId reservationId,
        string paymentIntentId,
        string status,
        decimal amount,
        string currency,
        string? failureReason,
        CancellationToken ct);
}
