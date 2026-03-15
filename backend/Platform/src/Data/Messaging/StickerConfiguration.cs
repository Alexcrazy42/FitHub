using FitHub.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Messaging;

public class StickerConfiguration : IEntityTypeConfiguration<Sticker>
{
    public void Configure(EntityTypeBuilder<Sticker> builder)
    {
        builder.HasOne(x => x.Group)
            .WithMany()
            .HasForeignKey(x => x.GroupId);

        builder.HasOne(x => x.File)
            .WithMany()
            .HasForeignKey(x => x.FileId);
    }
}
