using FitHub.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Messaging;

public class StickerGroupConfiguration : IEntityTypeConfiguration<StickerGroup>
{
    public void Configure(EntityTypeBuilder<StickerGroup> builder) { }
}
