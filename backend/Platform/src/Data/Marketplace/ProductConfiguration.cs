using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => new { x.CategoryId, x.IsActive });
        builder.Property(x => x.Name).HasMaxLength(255);
        builder.Property(x => x.Slug).HasMaxLength(255);
        builder.Property(x => x.Version).IsConcurrencyToken();

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);

        builder.HasOne(x => x.Brand)
            .WithMany()
            .HasForeignKey(x => x.BrandId);

        builder.HasMany(x => x.Images)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId);

        builder.HasMany(x => x.Variants)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId);

        builder.HasData(new
        {
            Id = MarketplaceDemoData.ProductId,
            CategoryId = MarketplaceDemoData.AccessoriesCategoryId,
            BrandId = MarketplaceDemoData.FitHubBrandId,
            Name = "FitHub Training Mat",
            Slug = "fithub-training-mat",
            Description = "Demo marketplace product for local development.",
            IsActive = true,
            CreatedAt = MarketplaceDemoData.CreatedAt,
            UpdatedAt = MarketplaceDemoData.CreatedAt,
            Version = 0L
        });
    }
}
