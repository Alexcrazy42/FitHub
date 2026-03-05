using FitHub.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Messaging;

public class GifConfiguration : IEntityTypeConfiguration<Gif>
{
    public void Configure(EntityTypeBuilder<Gif> builder)
    {
        builder.HasOne(x => x.File)
            .WithMany()
            .HasForeignKey(x => x.FileId);
    }
}
