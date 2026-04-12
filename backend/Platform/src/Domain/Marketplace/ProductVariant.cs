using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace;

public class ProductVariant : IEntity<ProductVariantId>
{
    private readonly List<ProductVariantAttribute> attributes = [];

    private ProductVariant(
        ProductVariantId id,
        ProductId productId,
        string sku,
        decimal priceAmount,
        string currency)
    {
        Id = id;
        ProductId = productId;
        Sku = sku;
        PriceAmount = priceAmount;
        Currency = currency;
        IsActive = true;
    }

    public ProductVariantId Id { get; }
    public ProductId ProductId { get; private set; }
    public Product? Product { get; private set; }
    public string Sku { get; private set; }
    public string? Name { get; private set; }
    public decimal PriceAmount { get; private set; }
    public string Currency { get; private set; }
    public decimal? CompareAtPriceAmount { get; private set; }
    public bool IsActive { get; private set; }
    public long Version { get; private set; }
    public ProductVariantInventory? Inventory { get; private set; }
    public IReadOnlyList<ProductVariantAttribute> Attributes => attributes;

    public void AddAttribute(ProductVariantAttribute attribute)
    {
        attributes.Add(attribute);
        Version++;
    }

    public void SetCompareAtPrice(decimal? compareAtPriceAmount)
    {
        CompareAtPriceAmount = compareAtPriceAmount;
        Version++;
    }

    public static ProductVariant Create(ProductId productId, string sku, decimal priceAmount, string currency = "RUB", string? name = null)
    {
        return new ProductVariant(ProductVariantId.New(), productId, sku, priceAmount, currency)
        {
            Name = name
        };
    }
}
