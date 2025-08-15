using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Trainings.BaseGroupTrainings;

public class BaseGroupTrainingConfiguration : IEntityTypeConfiguration<BaseGroupTraining>
{
    public void Configure(EntityTypeBuilder<BaseGroupTraining> builder)
    {
    }
}
