using FitHub.Domain.Marketplace;
using FitHub.Domain.Marketplace.Deliveries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class DeliveryEventConfiguration : IEntityTypeConfiguration<DeliveryEvent>
{
    public void Configure(EntityTypeBuilder<DeliveryEvent> builder)
    {
        builder.HasIndex(x => new { x.DeliveryId, x.CreatedAt });
        builder.Property(x => x.Message).HasMaxLength(1000);
    }
}
