using FitHub.Domain.Marketplace;
using FitHub.Domain.Marketplace.Deliveries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.HasIndex(x => x.OrderId).IsUnique();
        builder.HasIndex(x => x.Status);
        builder.Property(x => x.PickupAddress).HasMaxLength(1000);
        builder.Property(x => x.DropoffAddress).HasMaxLength(1000);
        builder.Property(x => x.LastAutomaticDecisionReason).HasMaxLength(1000);
        builder.HasIndex(x => x.CourierAssignmentExpiresAt);

        builder.HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId);

        builder.HasOne(x => x.Courier)
            .WithMany()
            .HasForeignKey(x => x.CourierId);

        builder.HasMany(x => x.TrackingPoints)
            .WithOne(x => x.Delivery)
            .HasForeignKey(x => x.DeliveryId);

        builder.HasMany(x => x.Events)
            .WithOne(x => x.Delivery)
            .HasForeignKey(x => x.DeliveryId);
    }
}
