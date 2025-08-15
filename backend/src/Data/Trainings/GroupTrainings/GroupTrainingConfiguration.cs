using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Trainings.GroupTrainings;

public class GroupTrainingConfiguration : IEntityTypeConfiguration<GroupTraining>
{
    public void Configure(EntityTypeBuilder<GroupTraining> builder)
    {
        builder.HasOne(x => x.BaseGroupTraining)
            .WithMany()
            .HasForeignKey(x => x.BaseGroupTrainingId);

        builder.HasOne(x => x.Trainer)
            .WithMany()
            .HasForeignKey(x => x.TrainerId);

        builder.HasMany(x => x.Participants)
            .WithMany(x => x.GroupTrainings);
    }
}
