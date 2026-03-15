using FitHub.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Users;

public class VisitorConfiguration : IEntityTypeConfiguration<Visitor>
{
    public void Configure(EntityTypeBuilder<Visitor> builder)
    {

    }
}
