using FitHub.Domain.Marketplace;
using FitHub.Domain.Marketplace.Deliveries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class CourierConfiguration : IEntityTypeConfiguration<Courier>
{
    public void Configure(EntityTypeBuilder<Courier> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(255);
        builder.HasIndex(x => x.IsAvailable);
    }
}
