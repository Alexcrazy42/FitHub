using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace;

public class MarketplaceOrderItem : IEntity<MarketplaceOrderItemId>
{
    private MarketplaceOrderItem(
        MarketplaceOrderItemId id,
        MarketplaceOrderId orderId,
        ProductId productId,
        ProductVariantId productVariantId,
        string productName,
        string sku,
        decimal unitPriceAmount,
        string currency,
        int quantity)
    {
        Id = id;
        OrderId = orderId;
        ProductId = productId;
        ProductVariantId = productVariantId;
        ProductName = productName;
        Sku = sku;
        UnitPriceAmount = unitPriceAmount;
        Currency = currency;
        Quantity = quantity;
    }

    public MarketplaceOrderItemId Id { get; }
    public MarketplaceOrderId OrderId { get; private set; }
    public MarketplaceOrder? Order { get; private set; }
    public ProductId ProductId { get; private set; }
    public ProductVariantId ProductVariantId { get; private set; }
    public string ProductName { get; private set; }
    public string? BrandName { get; private set; }
    public string Sku { get; private set; }
    public string? VariantName { get; private set; }
    public decimal UnitPriceAmount { get; private set; }
    public string Currency { get; private set; }
    public int Quantity { get; private set; }
    public string? ImageFileId { get; private set; }
    public string? AttributeSummary { get; private set; }
    public decimal TotalAmount => UnitPriceAmount * Quantity;

    public static MarketplaceOrderItem Create(
        MarketplaceOrderId orderId,
        ProductId productId,
        ProductVariantId productVariantId,
        string productName,
        string? brandName,
        string sku,
        string? variantName,
        decimal unitPriceAmount,
        string currency,
        int quantity,
        string? imageFileId,
        string? attributeSummary)
    {
        if (quantity <= 0)
        {
            throw new ValidationException("Order item quantity must be positive.");
        }

        if (unitPriceAmount <= 0)
        {
            throw new ValidationException("Order item price must be positive.");
        }

        return new MarketplaceOrderItem(
            MarketplaceOrderItemId.New(),
            orderId,
            productId,
            productVariantId,
            productName,
            sku,
            unitPriceAmount,
            currency,
            quantity)
        {
            BrandName = brandName,
            VariantName = variantName,
            ImageFileId = imageFileId,
            AttributeSummary = attributeSummary
        };
    }
}
