using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Marketplace;

public class StockReservation : IEntity<StockReservationId>
{
    private StockReservation(
        StockReservationId id,
        ProductVariantId productVariantId,
        int quantity,
        DateTimeOffset expiresAt,
        string idempotencyKey,
        IdentityUserId? createdByUserId,
        DateTimeOffset createdAt)
    {
        Id = id;
        ProductVariantId = productVariantId;
        Quantity = quantity;
        ExpiresAt = expiresAt;
        IdempotencyKey = idempotencyKey;
        CreatedByUserId = createdByUserId;
        CreatedAt = createdAt;
        Status = StockReservationStatus.Active;
    }

    public StockReservationId Id { get; }
    public ProductVariantId ProductVariantId { get; private set; }
    public ProductVariant? ProductVariant { get; private set; }
    public int Quantity { get; private set; }
    public StockReservationStatus Status { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public string IdempotencyKey { get; private set; }
    public IdentityUserId? CreatedByUserId { get; private set; }
    public User? CreatedByUser { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public void Expire()
    {
        if (Status == StockReservationStatus.Active)
        {
            Status = StockReservationStatus.Expired;
        }
    }

    public void Release()
    {
        if (Status == StockReservationStatus.Active)
        {
            Status = StockReservationStatus.Released;
        }
    }

    public static StockReservation Create(
        ProductVariantId productVariantId,
        int quantity,
        DateTimeOffset expiresAt,
        string idempotencyKey,
        IdentityUserId? createdByUserId = null)
    {
        return new StockReservation(
            StockReservationId.New(),
            productVariantId,
            quantity,
            expiresAt,
            idempotencyKey,
            createdByUserId,
            DateTimeOffset.UtcNow);
    }
}
