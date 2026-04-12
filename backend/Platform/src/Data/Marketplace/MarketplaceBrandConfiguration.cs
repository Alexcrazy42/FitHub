using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class MarketplaceBrandConfiguration : IEntityTypeConfiguration<MarketplaceBrand>
{
    public void Configure(EntityTypeBuilder<MarketplaceBrand> builder)
    {
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(255);
        builder.Property(x => x.Slug).HasMaxLength(255);

        builder.HasData(new
        {
            Id = MarketplaceDemoData.FitHubBrandId,
            Name = "FitHub",
            Slug = "fithub"
        });
    }
}
