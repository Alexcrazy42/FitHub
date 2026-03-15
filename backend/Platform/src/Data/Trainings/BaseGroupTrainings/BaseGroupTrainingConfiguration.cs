using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Trainings.BaseGroupTrainings;

public class BaseGroupTrainingConfiguration : IEntityTypeConfiguration<BaseGroupTraining>
{
    public void Configure(EntityTypeBuilder<BaseGroupTraining> builder)
    {
        builder.HasMany(x => x.TrainingTypes)
            .WithMany();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);
    }
}
