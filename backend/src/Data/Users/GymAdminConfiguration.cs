using FitHub.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Users;

public class GymAdminConfiguration : IEntityTypeConfiguration<GymAdmin>
{
    public void Configure(EntityTypeBuilder<GymAdmin> builder)
    {
        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<GymAdmin>(x => x.UserId);

        builder.HasMany(x => x.Gyms)
            .WithMany(x => x.Admins);
    }
}
