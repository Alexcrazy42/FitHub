using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasIndex(x => new { x.ProductId, x.SortOrder });
        builder.Property(x => x.AltText).HasMaxLength(512);

        builder.HasOne(x => x.File)
            .WithMany()
            .HasForeignKey(x => x.FileId);
    }
}
