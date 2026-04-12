using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class ProductVariantInventoryConfiguration : IEntityTypeConfiguration<ProductVariantInventory>
{
    public void Configure(EntityTypeBuilder<ProductVariantInventory> builder)
    {
        builder.HasIndex(x => x.ProductVariantId).IsUnique();
        builder.Ignore(x => x.AvailableQuantity);
        builder.Property(x => x.Version).IsConcurrencyToken();

        builder.HasData(new
        {
            Id = MarketplaceDemoData.ProductVariantInventoryId,
            ProductVariantId = MarketplaceDemoData.ProductVariantId,
            QuantityOnHand = 25,
            QuantityReserved = 0,
            Version = 0L
        });
    }
}
