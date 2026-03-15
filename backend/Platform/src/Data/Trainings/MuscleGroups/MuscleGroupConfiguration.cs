using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Trainings.MuscleGroups;

public class MuscleGroupConfiguration : IEntityTypeConfiguration<MuscleGroup>
{
    public void Configure(EntityTypeBuilder<MuscleGroup> builder)
    {
        builder.HasOne(m => m.Parent)
            .WithMany(m => m.Childrens)
            .HasForeignKey(m => m.ParentId);
    }
}
