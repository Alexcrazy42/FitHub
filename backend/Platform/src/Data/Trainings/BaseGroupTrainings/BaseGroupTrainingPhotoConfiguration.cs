using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Trainings.BaseGroupTrainings;

public class BaseGroupTrainingPhotoConfiguration : IEntityTypeConfiguration<BaseGroupTrainingPhoto>
{
    public void Configure(EntityTypeBuilder<BaseGroupTrainingPhoto> builder)
    {
        builder.HasOne(x => x.Training)
            .WithMany(x => x.Photos)
            .HasForeignKey(x => x.TrainingId);

        builder.HasOne(x => x.File)
            .WithMany()
            .HasForeignKey(x => x.FileId);

    }
}
