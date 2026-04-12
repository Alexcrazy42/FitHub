using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class AttributeOptionConfiguration : IEntityTypeConfiguration<AttributeOption>
{
    public void Configure(EntityTypeBuilder<AttributeOption> builder)
    {
        builder.HasIndex(x => new { x.AttributeDefinitionId, x.Value }).IsUnique();
        builder.Property(x => x.Value).HasMaxLength(255);

        builder.HasData(new
        {
            Id = MarketplaceDemoData.SizeMOptionId,
            AttributeDefinitionId = MarketplaceDemoData.SizeAttributeId,
            Value = "M",
            SortOrder = 0
        });
    }
}
