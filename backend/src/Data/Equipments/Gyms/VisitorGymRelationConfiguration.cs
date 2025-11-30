using FitHub.Domain.Equipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Equipments.Gyms;

public class VisitorGymRelationConfiguration : IEntityTypeConfiguration<VisitorGymRelation>
{
    public void Configure(EntityTypeBuilder<VisitorGymRelation> builder)
    {
        builder.HasKey(x => new { x.VisitorId, x.GymId });

        builder.HasOne(x => x.Visitor)
            .WithMany(visitor => visitor.Gyms)
            .HasForeignKey(x => x.VisitorId);

        builder.HasOne(x => x.Gym)
            .WithMany(g => g.Visitors)
            .HasForeignKey(x => x.GymId);
    }
}
