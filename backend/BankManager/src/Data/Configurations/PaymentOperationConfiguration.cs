using FitHub.BankManager.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.BankManager.Data.Configurations;

public class PaymentOperationConfiguration : IEntityTypeConfiguration<PaymentOperation>
{
    public void Configure(EntityTypeBuilder<PaymentOperation> builder)
    {
        builder.HasIndex(x => x.ExternalEventId);
        builder.Property(x => x.ExternalEventId).HasMaxLength(255);
        builder.Property(x => x.FailureReason).HasMaxLength(1000);
    }
}
