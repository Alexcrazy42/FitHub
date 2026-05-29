using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace;

public class ProductVariantInventory : IEntity<ProductVariantInventoryId>
{
    private ProductVariantInventory(ProductVariantInventoryId id, ProductVariantId productVariantId, int quantityOnHand)
    {
        Id = id;
        ProductVariantId = productVariantId;
        QuantityOnHand = quantityOnHand;
    }

    public ProductVariantInventoryId Id { get; }
    public ProductVariantId ProductVariantId { get; private set; }
    public ProductVariant? ProductVariant { get; private set; }
    public int QuantityOnHand { get; private set; }
    public int QuantityReserved { get; private set; }
    public int AvailableQuantity => QuantityOnHand - QuantityReserved;
    public long Version { get; private set; }

    public bool TryReserve(int quantity)
    {
        if (quantity <= 0 || AvailableQuantity < quantity)
        {
            return false;
        }

        QuantityReserved += quantity;
        Version++;
        return true;
    }

    public bool TryReleaseReserved(int quantity)
    {
        if (quantity <= 0 || QuantityReserved < quantity)
        {
            return false;
        }

        QuantityReserved -= quantity;
        Version++;
        return true;
    }

    public static ProductVariantInventory Create(ProductVariantId productVariantId, int quantityOnHand)
    {
        return new ProductVariantInventory(ProductVariantInventoryId.New(), productVariantId, quantityOnHand);
    }
}
