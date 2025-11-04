using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Trainings.VideoTrainings;

public class VideoTrainingConfiguration : IEntityTypeConfiguration<VideoTraining>
{
    public void Configure(EntityTypeBuilder<VideoTraining> builder)
    {
        builder.HasOne(x => x.TrainingType)
            .WithMany()
            .HasForeignKey(x => x.TrainingTypeId);
    }
}
