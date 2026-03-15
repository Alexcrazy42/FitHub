using FitHub.Domain.Equipments;
using FitHub.Domain.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Equipments.Gyms;

public class GymConfiguration : IEntityTypeConfiguration<Gym>
{
    public void Configure(EntityTypeBuilder<Gym> builder)
    {
        builder.HasMany(x => x.Admins)
            .WithMany(x => x.Gyms);

        builder.HasMany(x => x.GymEquipments)
            .WithOne(x => x.Gym);

        builder.HasMany(x => x.Zones)
            .WithMany(x => x.Gyms);

        builder.Ignore(x => x.Files);
    }
}
