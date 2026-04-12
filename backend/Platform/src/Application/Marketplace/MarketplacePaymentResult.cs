using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public record MarketplacePaymentResult(
    StockReservation Reservation,
    string? PaymentIntentId,
    string PaymentStatus,
    decimal Amount,
    string Currency,
    string? FailureReason);
