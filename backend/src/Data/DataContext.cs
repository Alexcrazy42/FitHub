using FitHub.Common.EntityFramework;
using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FitHub.Data;

public sealed class DataContext : DbContext
{

    public DataContext(
        DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<BaseGroupTraining> BaseGroupTrainings => Set<BaseGroupTraining>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        modelBuilder.UseCommonConventions();
    }

    /// <summary>
    /// Применить миграции
    /// </summary>
    public Task MigrateAsync()
        => Database.MigrateAsync();
}
