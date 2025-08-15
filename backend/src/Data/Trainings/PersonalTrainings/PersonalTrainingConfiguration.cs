using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Trainings.PersonalTrainings;

public class PersonalTrainingConfiguration : IEntityTypeConfiguration<PersonalTraining>
{
    public void Configure(EntityTypeBuilder<PersonalTraining> builder)
    {
        builder.HasOne(x => x.Visitor)
            .WithMany()
            .HasForeignKey(x => x.VisitorId);

        builder.HasOne(x => x.Trainer)
            .WithMany()
            .HasForeignKey(x => x.TrainerId);
    }
}
