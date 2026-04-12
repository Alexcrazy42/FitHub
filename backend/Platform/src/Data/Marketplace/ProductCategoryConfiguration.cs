using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(255);
        builder.Property(x => x.Slug).HasMaxLength(255);

        builder.HasOne(x => x.Parent)
            .WithMany()
            .HasForeignKey(x => x.ParentId);

        builder.HasData(new
        {
            Id = MarketplaceDemoData.AccessoriesCategoryId,
            Name = "Accessories",
            Slug = "accessories",
            ParentId = (ProductCategoryId?)null,
            IsActive = true
        });
    }
}
