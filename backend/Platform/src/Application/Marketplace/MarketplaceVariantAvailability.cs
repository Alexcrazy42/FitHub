using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public static class MarketplaceVariantAvailability
{
    public static bool IsAvailable(ProductVariant variant)
        => IsAvailable(
            variant.IsActive,
            variant.PriceAmount,
            variant.Inventory?.QuantityOnHand,
            variant.Inventory?.QuantityReserved);

    public static bool IsAvailable(bool isActive, decimal priceAmount, int? quantityOnHand, int? quantityReserved)
    {
        return isActive &&
               priceAmount > 0 &&
               quantityOnHand is not null &&
               quantityOnHand.Value - (quantityReserved ?? 0) > 0;
    }
}
