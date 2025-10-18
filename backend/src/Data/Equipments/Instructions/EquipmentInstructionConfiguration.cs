using FitHub.Domain.Equipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Equipments.Instructions;

public class EquipmentInstructionConfiguration : IEntityTypeConfiguration<EquipmentInstruction>
{
    public void Configure(EntityTypeBuilder<EquipmentInstruction> builder)
    {
        builder.HasOne(x => x.Equipment)
            .WithMany(x => x.Instructions);

        builder.HasMany(x => x.MuscleGroups)
            .WithMany();
    }
}
