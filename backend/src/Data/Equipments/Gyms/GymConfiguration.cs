using FitHub.Domain.Equipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Equipments.Gyms;

public class GymConfiguration : IEntityTypeConfiguration<Gym>
{
    public void Configure(EntityTypeBuilder<Gym> builder)
    {
        builder.HasMany(x => x.Admins)
            .WithMany(x => x.Gyms);

        builder.HasMany(x => x.Equipments)
            .WithMany(x => x.Gyms);
    }
}
