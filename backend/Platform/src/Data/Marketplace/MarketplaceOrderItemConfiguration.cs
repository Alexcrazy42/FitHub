using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class MarketplaceOrderItemConfiguration : IEntityTypeConfiguration<MarketplaceOrderItem>
{
    public void Configure(EntityTypeBuilder<MarketplaceOrderItem> builder)
    {
        builder.Property(x => x.ProductName).HasMaxLength(500);
        builder.Property(x => x.BrandName).HasMaxLength(255);
        builder.Property(x => x.Sku).HasMaxLength(128);
        builder.Property(x => x.VariantName).HasMaxLength(255);
        builder.Property(x => x.Currency).HasMaxLength(3);
        builder.Property(x => x.ImageFileId).HasMaxLength(255);
        builder.Property(x => x.AttributeSummary).HasMaxLength(1000);
        builder.Ignore(x => x.TotalAmount);
    }
}
