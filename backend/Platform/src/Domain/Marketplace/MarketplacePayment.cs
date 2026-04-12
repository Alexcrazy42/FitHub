using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace;

public class MarketplacePayment : IEntity<MarketplacePaymentId>
{
    private MarketplacePayment(
        MarketplacePaymentId id,
        StockReservationId reservationId,
        decimal amount,
        string currency,
        string idempotencyKey)
    {
        Id = id;
        ReservationId = reservationId;
        Amount = amount;
        Currency = currency;
        IdempotencyKey = idempotencyKey;
        Status = MarketplacePaymentStatus.Processing;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public MarketplacePaymentId Id { get; }
    public StockReservationId ReservationId { get; private set; }
    public StockReservation? Reservation { get; private set; }
    public string? BankPaymentIntentId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public MarketplacePaymentStatus Status { get; private set; }
    public string IdempotencyKey { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long Version { get; private set; }

    public void ApplyBankStatus(string bankPaymentIntentId, string status, string? failureReason)
    {
        BankPaymentIntentId = bankPaymentIntentId;
        FailureReason = failureReason;

        Status = status switch
        {
            "Paid" => MarketplacePaymentStatus.Paid,
            "Failed" => MarketplacePaymentStatus.Failed,
            "Expired" => MarketplacePaymentStatus.Expired,
            _ => MarketplacePaymentStatus.Processing
        };

        Touch();
    }

    public void MarkExpired(string reason)
    {
        Status = MarketplacePaymentStatus.Expired;
        FailureReason = reason;
        Touch();
    }

    private void Touch()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
        Version++;
    }

    public static MarketplacePayment Create(StockReservationId reservationId, decimal amount, string currency, string idempotencyKey)
    {
        if (amount <= 0)
        {
            throw new ValidationException("Payment amount must be positive.");
        }

        if (String.IsNullOrWhiteSpace(currency))
        {
            throw new ValidationException("Currency is required.");
        }

        if (String.IsNullOrWhiteSpace(idempotencyKey))
        {
            throw new ValidationException("IdempotencyKey is required.");
        }

        return new MarketplacePayment(MarketplacePaymentId.New(), reservationId, amount, currency, idempotencyKey);
    }
}
