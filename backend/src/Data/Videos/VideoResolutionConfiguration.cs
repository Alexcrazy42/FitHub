using FitHub.Domain.Videos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Videos;

public class VideoResolutionConfiguration : IEntityTypeConfiguration<VideoResolution>
{
    public void Configure(EntityTypeBuilder<VideoResolution> builder)
    {
        builder.ToTable("VideoResolutions");
    }
}
