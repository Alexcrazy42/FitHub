using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasIndex(x => new { x.ProductId, x.Sku }).IsUnique();
        builder.HasIndex(x => new { x.ProductId, x.IsActive });
        builder.Property(x => x.Sku).HasMaxLength(128);
        builder.Property(x => x.Name).HasMaxLength(255);
        builder.Property(x => x.Currency).HasMaxLength(3);
        builder.Property(x => x.PriceAmount).HasPrecision(18, 2);
        builder.Property(x => x.CompareAtPriceAmount).HasPrecision(18, 2);
        builder.Property(x => x.Version).IsConcurrencyToken();

        builder.HasMany(x => x.Attributes)
            .WithOne(x => x.ProductVariant)
            .HasForeignKey(x => x.ProductVariantId);

        builder.HasOne(x => x.Inventory)
            .WithOne(x => x.ProductVariant)
            .HasForeignKey<ProductVariantInventory>(x => x.ProductVariantId);

        builder.HasData(new
        {
            Id = MarketplaceDemoData.ProductVariantId,
            ProductId = MarketplaceDemoData.ProductId,
            Sku = "FITHUB-MAT-M",
            Name = "Medium",
            PriceAmount = 2490m,
            Currency = "RUB",
            CompareAtPriceAmount = (decimal?)null,
            IsActive = true,
            Version = 0L
        });
    }
}
