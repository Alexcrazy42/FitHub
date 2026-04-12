using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class ProductVariantAttributeConfiguration : IEntityTypeConfiguration<ProductVariantAttribute>
{
    public void Configure(EntityTypeBuilder<ProductVariantAttribute> builder)
    {
        builder.HasIndex(x => new { x.ProductVariantId, x.AttributeDefinitionId }).IsUnique();

        builder.HasOne(x => x.AttributeDefinition)
            .WithMany()
            .HasForeignKey(x => x.AttributeDefinitionId);

        builder.HasOne(x => x.AttributeOption)
            .WithMany()
            .HasForeignKey(x => x.AttributeOptionId);

        builder.HasData(new
        {
            Id = MarketplaceDemoData.ProductVariantAttributeId,
            ProductVariantId = MarketplaceDemoData.ProductVariantId,
            AttributeDefinitionId = MarketplaceDemoData.SizeAttributeId,
            AttributeOptionId = MarketplaceDemoData.SizeMOptionId
        });
    }
}
