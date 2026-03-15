using FitHub.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Users;

public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
{
    public void Configure(EntityTypeBuilder<Trainer> builder)
    {
        builder.HasMany(x => x.Gyms)
            .WithMany(x => x.Trainers);
    }
}
