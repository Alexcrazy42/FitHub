using FitHub.Domain.Equipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Equipments.Gyms;

public class GymZoneConfiguration : IEntityTypeConfiguration<GymZone>
{
    public void Configure(EntityTypeBuilder<GymZone> builder)
    {

    }
}
