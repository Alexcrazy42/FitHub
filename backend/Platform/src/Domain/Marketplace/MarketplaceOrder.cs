using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace;

public class MarketplaceOrder : IEntity<MarketplaceOrderId>
{
    private readonly List<MarketplaceOrderItem> items = [];
    private readonly List<MarketplaceOrderStatusHistory> statusHistory = [];

    private MarketplaceOrder(
        MarketplaceOrderId id,
        StockReservationId reservationId,
        MarketplacePaymentId paymentId,
        decimal totalAmount,
        string currency,
        DateTimeOffset createdAt)
    {
        Id = id;
        ReservationId = reservationId;
        PaymentId = paymentId;
        TotalAmount = totalAmount;
        Currency = currency;
        Status = MarketplaceOrderStatus.Created;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public MarketplaceOrderId Id { get; }
    public StockReservationId ReservationId { get; private set; }
    public StockReservation? Reservation { get; private set; }
    public MarketplacePaymentId PaymentId { get; private set; }
    public MarketplacePayment? Payment { get; private set; }
    public MarketplaceOrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long Version { get; private set; }
    public IReadOnlyList<MarketplaceOrderItem> Items => items;
    public IReadOnlyList<MarketplaceOrderStatusHistory> StatusHistory => statusHistory;

    public static MarketplaceOrder CreateFromPaidReservation(StockReservation reservation, MarketplacePayment payment)
    {
        if (reservation.Status != StockReservationStatus.Paid)
        {
            throw new ValidationException("Only paid reservation can be converted to order.");
        }

        if (payment.Status != MarketplacePaymentStatus.Paid)
        {
            throw new ValidationException("Only paid payment can be converted to order.");
        }

        if (reservation.ProductVariant is null)
        {
            throw new ValidationException("Reservation variant is required to create order.");
        }

        var variant = reservation.ProductVariant;
        var product = variant.Product;
        var currency = payment.Currency;
        var unitPriceAmount = payment.Amount / reservation.Quantity;
        var order = new MarketplaceOrder(
            MarketplaceOrderId.New(),
            reservation.Id,
            payment.Id,
            payment.Amount,
            currency,
            DateTimeOffset.UtcNow);

        order.items.Add(MarketplaceOrderItem.Create(
            order.Id,
            variant.ProductId,
            variant.Id,
            product?.Name ?? variant.Sku,
            product?.Brand?.Name,
            variant.Sku,
            variant.Name,
            unitPriceAmount,
            currency,
            reservation.Quantity,
            product?.Images
                .OrderByDescending(x => x.IsMain)
                .ThenBy(x => x.SortOrder)
                .FirstOrDefault()
                ?.FileId
                .ToString(),
            FormatAttributes(variant.Attributes)));

        order.statusHistory.Add(MarketplaceOrderStatusHistory.Create(
            order.Id,
            order.Status,
            "Order created from paid marketplace reservation."));

        return order;
    }

    private static string? FormatAttributes(IReadOnlyList<ProductVariantAttribute> attributes)
    {
        var values = attributes
            .OrderBy(x => x.AttributeDefinition?.SortOrder ?? 0)
            .ThenBy(x => x.AttributeDefinition?.Name)
            .Select(x => (Name: x.AttributeDefinition?.Name, Value: x.AttributeOption?.Value))
            .Where(x => !String.IsNullOrWhiteSpace(x.Name) && !String.IsNullOrWhiteSpace(x.Value))
            .Select(x => $"{x.Name}: {x.Value}")
            .ToList();

        return values.Count == 0 ? null : String.Join(", ", values);
    }
}
