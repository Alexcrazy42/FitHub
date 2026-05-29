using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Entities;

public class LogConfiguration : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.Property(x => x.Payload)
            .HasColumnType("jsonb");

        builder.Property(x => x.Created)
            .HasDefaultValueSql("now()");
    }
}