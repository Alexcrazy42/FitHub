using FitHub.Domain.Equipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Equipments.GymEquipments;

public class GymEquipmentConfiguration : IEntityTypeConfiguration<GymEquipment>
{
    public void Configure(EntityTypeBuilder<GymEquipment> builder)
    {
        builder.HasOne(x => x.Equipment)
            .WithMany()
            .HasForeignKey(x => x.EquipmentId);

        builder.HasOne(x => x.Gym)
            .WithMany(gym => gym.GymEquipments)
            .HasForeignKey(x => x.GymId);
    }
}
