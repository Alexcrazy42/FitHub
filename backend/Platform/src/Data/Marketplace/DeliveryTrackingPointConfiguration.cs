using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class DeliveryTrackingPointConfiguration : IEntityTypeConfiguration<DeliveryTrackingPoint>
{
    public void Configure(EntityTypeBuilder<DeliveryTrackingPoint> builder)
    {
        builder.HasIndex(x => new { x.DeliveryId, x.CreatedAt });
        builder.Property(x => x.Latitude).HasPrecision(9, 6);
        builder.Property(x => x.Longitude).HasPrecision(9, 6);
    }
}
