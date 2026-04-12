using FitHub.BankManager.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.BankManager.Data.Configurations;

public class BankWebhookEventConfiguration : IEntityTypeConfiguration<BankWebhookEvent>
{
    public void Configure(EntityTypeBuilder<BankWebhookEvent> builder)
    {
        builder.HasIndex(x => x.ExternalEventId).IsUnique();
        builder.Property(x => x.ExternalEventId).HasMaxLength(255);
        builder.Property(x => x.Payload).HasMaxLength(4000);

        builder.HasOne(x => x.PaymentIntent)
            .WithMany()
            .HasForeignKey(x => x.PaymentIntentId);
    }
}
