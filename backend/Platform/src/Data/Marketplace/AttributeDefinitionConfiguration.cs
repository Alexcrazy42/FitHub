using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class AttributeDefinitionConfiguration : IEntityTypeConfiguration<AttributeDefinition>
{
    public void Configure(EntityTypeBuilder<AttributeDefinition> builder)
    {
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Code).HasMaxLength(128);
        builder.Property(x => x.Name).HasMaxLength(255);

        builder.HasMany(x => x.Options)
            .WithOne(x => x.AttributeDefinition)
            .HasForeignKey(x => x.AttributeDefinitionId);

        builder.HasData(new
        {
            Id = MarketplaceDemoData.SizeAttributeId,
            Code = "size",
            Name = "Size",
            IsPurchaseOption = true,
            IsFilterable = true,
            SortOrder = 0
        });
    }
}
