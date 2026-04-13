using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace;

public class MarketplaceOrderStatusHistory : IEntity<MarketplaceOrderStatusHistoryId>
{
    private MarketplaceOrderStatusHistory(
        MarketplaceOrderStatusHistoryId id,
        MarketplaceOrderId orderId,
        MarketplaceOrderStatus status,
        DateTimeOffset createdAt)
    {
        Id = id;
        OrderId = orderId;
        Status = status;
        CreatedAt = createdAt;
    }

    public MarketplaceOrderStatusHistoryId Id { get; }
    public MarketplaceOrderId OrderId { get; private set; }
    public MarketplaceOrder? Order { get; private set; }
    public MarketplaceOrderStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public string? Reason { get; private set; }

    public static MarketplaceOrderStatusHistory Create(
        MarketplaceOrderId orderId,
        MarketplaceOrderStatus status,
        string? reason = null)
    {
        return new MarketplaceOrderStatusHistory(
            MarketplaceOrderStatusHistoryId.New(),
            orderId,
            status,
            DateTimeOffset.UtcNow)
        {
            Reason = reason
        };
    }
}
