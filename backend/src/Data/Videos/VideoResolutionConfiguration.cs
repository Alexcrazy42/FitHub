using FitHub.Domain.Videos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Videos;

public class VideoResolutionConfiguration : IEntityTypeConfiguration<VideoResolution>
{
    public void Configure(EntityTypeBuilder<VideoResolution> builder)
    {
        builder.HasOne(x => x.Video)
            .WithMany(video => video.Resolutions)
            .HasForeignKey(x => x.VideoId);
    }
}
