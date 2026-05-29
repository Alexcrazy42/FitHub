using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Marketplace;

public class MarketplaceOrderStatusHistoryConfiguration : IEntityTypeConfiguration<MarketplaceOrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<MarketplaceOrderStatusHistory> builder)
    {
        builder.Property(x => x.Reason).HasMaxLength(1000);
    }
}
