using FitHub.Domain.Files;
using FitHub.Domain.Videos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Videos;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.HasOne(v => v.OriginalFile)
            .WithMany()
            .HasForeignKey(v => v.OriginalFileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(v => v.Resolutions)
            .WithOne()
            .HasForeignKey(r => r.VideoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
