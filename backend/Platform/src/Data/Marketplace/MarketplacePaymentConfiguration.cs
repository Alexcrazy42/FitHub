using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class MarketplacePaymentConfiguration : IEntityTypeConfiguration<MarketplacePayment>
{
    public void Configure(EntityTypeBuilder<MarketplacePayment> builder)
    {
        builder.HasIndex(x => x.ReservationId).IsUnique();
        builder.HasIndex(x => x.IdempotencyKey).IsUnique();
        builder.Property(x => x.IdempotencyKey).HasMaxLength(255);
        builder.Property(x => x.BankPaymentIntentId).HasMaxLength(255);
        builder.Property(x => x.Currency).HasMaxLength(3);
        builder.Property(x => x.FailureReason).HasMaxLength(1000);
        builder.Property(x => x.Version).IsConcurrencyToken();

        builder.HasOne(x => x.Reservation)
            .WithMany()
            .HasForeignKey(x => x.ReservationId);
    }
}
