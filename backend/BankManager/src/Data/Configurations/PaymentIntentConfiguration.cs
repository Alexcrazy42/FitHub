using FitHub.BankManager.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.BankManager.Data.Configurations;

public class PaymentIntentConfiguration : IEntityTypeConfiguration<PaymentIntent>
{
    public void Configure(EntityTypeBuilder<PaymentIntent> builder)
    {
        builder.HasIndex(x => x.IdempotencyKey).IsUnique();
        builder.HasIndex(x => x.ExternalReference);
        builder.Property(x => x.ExternalReference).HasMaxLength(255);
        builder.Property(x => x.Currency).HasMaxLength(3);
        builder.Property(x => x.IdempotencyKey).HasMaxLength(255);
        builder.Property(x => x.FailureReason).HasMaxLength(1000);
        builder.Property(x => x.Version).IsConcurrencyToken();

        builder.HasOne(x => x.BankAccount)
            .WithMany()
            .HasForeignKey(x => x.BankAccountId);

        builder.HasMany(x => x.Operations)
            .WithOne(x => x.PaymentIntent)
            .HasForeignKey(x => x.PaymentIntentId);

        builder.Navigation(x => x.Operations).HasField("operations");
    }
}
