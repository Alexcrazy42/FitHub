using FitHub.Domain.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Outbox;

public class RabbitOutboxMessageConfiguration : IEntityTypeConfiguration<RabbitOutboxMessage>
{
    public void Configure(EntityTypeBuilder<RabbitOutboxMessage> builder)
    {
        builder.HasIndex(x => new { x.Status, x.CreatedAt });
        builder.Property(x => x.ExchangeName).HasMaxLength(255);
        builder.Property(x => x.RoutingKey).HasMaxLength(255);
        builder.Property(x => x.Payload).HasMaxLength(4000);
        builder.Property(x => x.Error).HasMaxLength(2000);
    }
}
