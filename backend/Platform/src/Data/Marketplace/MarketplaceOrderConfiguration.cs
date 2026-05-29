using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class MarketplaceOrderConfiguration : IEntityTypeConfiguration<MarketplaceOrder>
{
    public void Configure(EntityTypeBuilder<MarketplaceOrder> builder)
    {
        builder.HasIndex(x => x.ReservationId).IsUnique();
        builder.HasIndex(x => x.PaymentId).IsUnique();
        builder.Property(x => x.Currency).HasMaxLength(3);
        builder.Property(x => x.Version).IsConcurrencyToken();

        builder.HasOne(x => x.Reservation)
            .WithMany()
            .HasForeignKey(x => x.ReservationId);

        builder.HasOne(x => x.Payment)
            .WithMany()
            .HasForeignKey(x => x.PaymentId);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId);

        builder.HasMany(x => x.StatusHistory)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId);
    }
}
